using Code.Gameplay.Teacher.Configs;
using Code.Infrastructure.ConfigsManagement;

namespace Code.Gameplay.Teacher.Services
{
	public interface ITeacherConfigsService : IConfigsService
	{
		TeacherConfig TeacherConfig { get; }
	}
}
