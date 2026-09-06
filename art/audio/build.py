#!/usr/bin/env python3

import math
import random
import re
import struct
import subprocess
import sys
import tempfile
import wave
from pathlib import Path


ROOT = Path(__file__).resolve().parents[2]
SOURCES = ROOT / "sounds"
OUTPUT = ROOT / "src/gamejam-template/Assets/AddressableResources/Content/Audio"
SAMPLE_RATE = 44100
SFX_NORMALIZE = "loudnorm=I=-16:LRA=7:TP=-6"
MUSIC_NORMALIZE = "loudnorm=I=-18:LRA=5:TP=-9,volume=-9dB"


def run(*arguments):
	subprocess.run(arguments, check=True)


def ffmpeg(*arguments):
	run("ffmpeg", "-hide_banner", "-loglevel", "error", "-y", *arguments)


def require_audio(path):
	result = subprocess.run(
		("ffprobe", "-v", "error", "-show_entries", "format=duration", "-of", "csv=p=0", str(path)),
		check=True,
		capture_output=True,
		text=True,
	)
	if result.stdout.strip() in ("", "N/A"):
		raise RuntimeError(f"Speech synthesis produced no audio: {path}")


def normalize_peak(path, target):
	result = subprocess.run(
		("ffmpeg", "-hide_banner", "-nostats", "-i", str(path), "-af", "volumedetect", "-f", "null", "-"),
		check=True,
		capture_output=True,
		text=True,
	)
	match = re.search(r"max_volume: (-?[0-9.]+) dB", result.stderr)
	if match is None:
		raise RuntimeError(f"Could not measure peak level: {path}")

	gain = target - float(match.group(1))
	if abs(gain) < 0.05:
		return

	temporary = path.with_name(path.stem + ".normalized.wav")
	ffmpeg(
		"-i", str(path), "-af", f"volume={gain}dB", "-ar", str(SAMPLE_RATE),
		"-ac", "2" if "/Music/" in str(path) else "1", "-c:a", "pcm_s16le", str(temporary),
	)
	temporary.replace(path)


def normalize_existing():
	for path in OUTPUT.glob("*/*.wav"):
		normalize_peak(path, -18.0 if "/Music/" in str(path) else -6.0)


def render(source, output, audio_filter, channels=1):
	output.parent.mkdir(parents=True, exist_ok=True)
	ffmpeg(
		"-i", str(source),
		"-af", f"{audio_filter},{SFX_NORMALIZE}",
		"-ar", str(SAMPLE_RATE),
		"-ac", str(channels),
		"-c:a", "pcm_s16le",
		str(output),
	)


def write_wave(path, duration, sample_at):
	path.parent.mkdir(parents=True, exist_ok=True)
	with wave.open(str(path), "wb") as output:
		output.setnchannels(1)
		output.setsampwidth(2)
		output.setframerate(SAMPLE_RATE)
		frames = bytearray()
		for sample_index in range(round(duration * SAMPLE_RATE)):
			time = sample_index / SAMPLE_RATE
			value = max(-1.0, min(1.0, sample_at(time)))
			frames.extend(struct.pack("<h", round(value * 32767)))
		output.writeframes(frames)


def noise_burst(time, start, duration, frequency, randomizer):
	progress = (time - start) / duration
	if progress < 0.0 or progress >= 1.0:
		return 0.0
	envelope = math.sin(math.pi * progress) * (1.0 - progress)
	tone = math.sin(2.0 * math.pi * frequency * time)
	return (tone * 0.55 + randomizer.uniform(-1.0, 1.0) * 0.45) * envelope


def synthesize_missing(temporary):
	randomizer = random.Random(9427)

	write_wave(
		temporary / "duck_throw.wav",
		0.52,
		lambda time: math.sin(2.0 * math.pi * (780.0 * time + 1850.0 * time * time) * time)
			* math.sin(math.pi * time / 0.52) * 0.75,
	)

	write_wave(
		temporary / "duck_land.wav",
		0.34,
		lambda time: noise_burst(time, 0.01, 0.2, 95.0, randomizer)
			+ noise_burst(time, 0.06, 0.24, 52.0, randomizer) * 0.6,
	)

	write_wave(
		temporary / "answer_copied.wav",
		0.42,
		lambda time: noise_burst(time, 0.025, 0.16, 72.0, randomizer)
			+ noise_burst(time, 0.12, 0.18, 145.0, randomizer) * 0.45,
	)

	write_wave(
		temporary / "clock_tick.wav",
		1.0,
		lambda time: noise_burst(time, 0.0, 0.045, 2200.0, randomizer)
			+ noise_burst(time, 0.5, 0.04, 1700.0, randomizer) * 0.7,
	)

	write_wave(
		temporary / "teacher_footsteps.wav",
		1.2,
		lambda time: noise_burst(time, 0.06, 0.18, 84.0, randomizer)
			+ noise_burst(time, 0.66, 0.18, 74.0, randomizer) * 0.9,
	)

	write_wave(
		temporary / "normal_ticks.wav",
		60.0,
		lambda time: noise_burst(time % 1.0, 0.0, 0.035, 1900.0, randomizer) * 0.18,
	)

	write_wave(
		temporary / "urgent_ticks.wav",
		60.0,
		lambda time: noise_burst(time % 0.6, 0.0, 0.032, 2100.0, randomizer) * 0.2,
	)


def synthesize_speech(temporary):
	run("say", "-v", "Samantha", "-r", "260", "-o", str(temporary / "laugh_one.aiff"), "Ha ha ha!")
	run("say", "-v", "Daniel", "-r", "275", "-o", str(temporary / "laugh_two.aiff"), "Ha! Ha ha!")
	run("say", "-v", "Karen", "-r", "250", "-o", str(temporary / "laugh_three.aiff"), "Ha ha!")
	run(
		"say", "-v", "Lesya", "-r", "215", "-o", str(temporary / "intro_ukrainian.aiff"),
		"Контрольна. Правило перше: не списувати. Правило друге: не м'явкати.",
	)
	run(
		"say", "-v", "Samantha", "-r", "205", "-o", str(temporary / "time_warning.aiff"),
		"Ten minutes left, class!",
	)
	for path in temporary.glob("*.aiff"):
		require_audio(path)


def build_sfx(temporary):
	effects = OUTPUT / "Effects"
	render(SOURCES / "meow_3.mp3", effects / "meow_1.wav", "atrim=duration=1.1,afade=t=out:st=0.8:d=0.2")
	render(
		SOURCES / "meow_3.mp3", effects / "meow_2.wav",
		"asetrate=48000,aresample=44100,atrim=duration=1.0,afade=t=out:st=0.75:d=0.2",
	)
	render(
		SOURCES / "meow_3.mp3", effects / "meow_3.wav",
		"asetrate=40500,aresample=44100,atrim=duration=1.2,afade=t=out:st=0.95:d=0.2",
	)
	render(SOURCES / "duck.mp3", effects / "duck_squeak.wav", "atrim=duration=0.5")
	render(temporary / "duck_throw.wav", effects / "duck_throw.wav", "afade=t=out:st=0.42:d=0.08")
	render(temporary / "duck_land.wav", effects / "duck_land.wav", "afade=t=out:st=0.28:d=0.05")
	render(SOURCES / "error_4.mp3", effects / "pencil_snap.wav", "highpass=f=180,atrim=duration=0.45")
	render(
		SOURCES / "paper_writing.mp3", effects / "pencil_scratch.wav",
		"atrim=start=2.2:duration=0.42,asetpts=PTS-STARTPTS,highpass=f=700",
	)
	render(temporary / "answer_copied.wav", effects / "answer_copied.wav", "lowpass=f=5000")
	render(
		SOURCES / "paper_writing.mp3", effects / "chalk_scratch.wav",
		"atrim=start=10:duration=1.15,asetpts=PTS-STARTPTS,highpass=f=1300",
	)
	render(SOURCES / "reaction_1.mp3", effects / "teacher_hmm.wav", "atrim=duration=1.2")
	render(SOURCES / "heartbeat_2.mp3", effects / "heartbeat.wav", "atrim=duration=31.8")
	render(temporary / "clock_tick.wav", effects / "clock_tick.wav", "anull")
	render(SOURCES / "school_ring.wav", effects / "school_bell.wav", "atrim=duration=3.7")
	render(temporary / "teacher_footsteps.wav", effects / "teacher_footsteps.wav", "anull")
	render(temporary / "time_warning.aiff", effects / "time_warning.wav", "highpass=f=240,lowpass=f=3600")
	ffmpeg(
		"-i", str(temporary / "laugh_one.aiff"),
		"-i", str(temporary / "laugh_two.aiff"),
		"-i", str(temporary / "laugh_three.aiff"),
		"-filter_complex",
		"[0:a]adelay=0|0,volume=0.8[a];[1:a]adelay=90|90,volume=0.75[b];"
		"[2:a]adelay=170|170,volume=0.7[c];[a][b][c]amix=inputs=3:normalize=0,"
		f"highpass=f=180,lowpass=f=6000,{SFX_NORMALIZE}[out]",
		"-map", "[out]", "-ar", str(SAMPLE_RATE), "-ac", "1", "-c:a", "pcm_s16le",
		str(effects / "class_laugh.wav"),
	)


def build_music(temporary):
	music = OUTPUT / "Music"
	music.mkdir(parents=True, exist_ok=True)
	ffmpeg(
		"-i", str(SOURCES / "ingame_1.wav"),
		"-i", str(temporary / "normal_ticks.wav"),
		"-filter_complex",
		f"[0:a]atrim=duration=60,volume=0.8[room];[1:a]volume=0.8[ticks];"
		f"[room][ticks]amix=inputs=2:normalize=0,{MUSIC_NORMALIZE}[out]",
		"-map", "[out]", "-t", "60", "-ar", str(SAMPLE_RATE), "-ac", "2", "-c:a", "pcm_s16le",
		str(music / "classroom.wav"),
	)
	ffmpeg(
		"-i", str(SOURCES / "ingame_1.wav"),
		"-stream_loop", "-1", "-i", str(SOURCES / "tention_1.mp3"),
		"-i", str(temporary / "urgent_ticks.wav"),
		"-filter_complex",
		f"[0:a]atrim=duration=60,volume=0.72[room];[1:a]atrim=duration=60,volume=0.16[tension];"
		f"[2:a]volume=0.9[ticks];[room][tension][ticks]amix=inputs=3:normalize=0,{MUSIC_NORMALIZE}[out]",
		"-map", "[out]", "-t", "60", "-ar", str(SAMPLE_RATE), "-ac", "2", "-c:a", "pcm_s16le",
		str(music / "classroom_urgent.wav"),
	)


def build_supplemental():
	effects = OUTPUT / "Effects"
	music = OUTPUT / "Music"
	OUTPUT.mkdir(parents=True, exist_ok=True)
	render(SOURCES / "button_1.wav", effects / "ui_click.wav", "apad=pad_dur=0.06,atrim=duration=0.075")
	render(SOURCES / "win_2.mp3", effects / "result_passed.wav", "atrim=duration=2.05")
	render(SOURCES / "angrycat.mp3", effects / "result_caught.wav", "atrim=duration=3.2")
	render(SOURCES / "fail_6.mp3", effects / "result_bell.wav", "atrim=duration=1.95")
	music.mkdir(parents=True, exist_ok=True)
	ffmpeg(
		"-i", str(SOURCES / "main menu_3.wav"),
		"-af", f"atrim=duration=60,{MUSIC_NORMALIZE}",
		"-t", "60", "-ar", str(SAMPLE_RATE), "-ac", "2", "-c:a", "pcm_s16le",
		str(music / "main_menu.wav"),
	)

	for path in (
		effects / "ui_click.wav",
		effects / "result_passed.wav",
		effects / "result_caught.wav",
		effects / "result_bell.wav",
		music / "main_menu.wav",
	):
		normalize_peak(path, -18.0 if path.parent == music else -6.0)


def build_voice_over(temporary):
	voice_over = OUTPUT / "VoiceOver"
	voice_over.mkdir(parents=True, exist_ok=True)
	render(
		SOURCES / "meow_3.mp3", voice_over / "intro_panel_1.wav",
		"asetrate=39000,aresample=44100,volume=0.8,atrim=duration=1.2",
	)
	render(
		SOURCES / "meow_3.mp3", voice_over / "intro_panel_3.wav",
		"asetrate=46500,aresample=44100,volume=0.65,atrim=duration=1.0",
	)
	ffmpeg(
		"-stream_loop", "4", "-i", str(SOURCES / "meow_3.mp3"),
		"-i", str(temporary / "intro_ukrainian.aiff"),
		"-filter_complex",
		"[0:a]atrim=duration=7,volume=0.16,highpass=f=250,lowpass=f=4200[meow];"
		f"[1:a]volume=0.9,highpass=f=300,lowpass=f=3400[ua];[meow][ua]amix=inputs=2:normalize=0,"
		f"{SFX_NORMALIZE}[out]",
		"-map", "[out]", "-ar", str(SAMPLE_RATE), "-ac", "1", "-c:a", "pcm_s16le",
		str(voice_over / "intro_panel_2.wav"),
	)
	ffmpeg(
		"-stream_loop", "1", "-i", str(SOURCES / "meow_3.mp3"),
		"-af", f"asetrate=43000,aresample=44100,atrim=duration=2.0,{SFX_NORMALIZE}",
		"-ar", str(SAMPLE_RATE), "-ac", "1", "-c:a", "pcm_s16le",
		str(voice_over / "intro_panel_4.wav"),
	)


def main():
	OUTPUT.mkdir(parents=True, exist_ok=True)

	with tempfile.TemporaryDirectory(prefix="copycat-audio-") as directory:
		temporary = Path(directory)
		synthesize_missing(temporary)
		synthesize_speech(temporary)
		build_sfx(temporary)
		build_music(temporary)
		build_supplemental()
		build_voice_over(temporary)
		normalize_existing()


if __name__ == "__main__":
	if "--normalize-existing" in sys.argv:
		normalize_existing()
	elif "--supplemental" in sys.argv:
		build_supplemental()
	else:
		main()
