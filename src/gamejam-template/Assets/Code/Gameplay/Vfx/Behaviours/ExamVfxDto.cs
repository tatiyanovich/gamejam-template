using Code.Gameplay.Duck.Queries;
using Code.Gameplay.Exam.Queries;
using Code.Gameplay.Input.Queries;
using Code.Gameplay.Teacher.Queries;

namespace Code.Gameplay.Vfx.Behaviours
{
	public struct ExamVfxDto
	{
		public IExamQuery Exam;
		public ITeacherQuery Teacher;
		public IInputQuery Input;
		public IDuckQuery Duck;
	}
}
