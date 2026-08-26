using System;
using System.Collections.Generic;
using Framework.Essentials.DependencyInjection;
using Framework.Storage.Strategies;
using Zenject;

namespace Framework.Storage
{
	public class SaveManagementInstaller : PlainAbstractInstaller
	{
		private readonly List<Type> _saveDataTypes;

		/// <summary>
		/// Installer for save management.
		/// </summary>
		/// <param name="container">Zenject container for dependency injection.</param>
		/// <param name="saveDataTypes">Save data types (must implement <see cref="ISaveFile"/>) to save/load.</param>
		public SaveManagementInstaller(DiContainer container, List<Type> saveDataTypes) : base(container)
		{
			_saveDataTypes = saveDataTypes;
		}

		public override void InstallBindings()
		{
#if UNITY_EDITOR
			ValidateTypes();
			Container.Bind<ISaveLoadStrategy>().To<EditorFileSaveLoadStrategy>().AsSingle();
#else
			Container.Bind<ISaveLoadStrategy>().To<PlayerPrefsSaveLoadStrategy>().AsSingle();
#endif
			Container.Bind<ISaveLoadService>().To<SaveLoadService>().AsSingle().WithArguments(_saveDataTypes);
		}

		private void ValidateTypes()
		{
			foreach (Type type in _saveDataTypes)
			{
				if (typeof(ISaveFile).IsAssignableFrom(type) == false)
					throw new ArgumentException($"Type {type} does not implement {nameof(ISaveFile)}");
			}
		}
	}
}
