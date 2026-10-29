using System;
using System.IO;
using UnityEngine;

namespace RacingProject.SaveSystem
{
    public static class CareerSaveSystem
    {
        private const string SaveFileName = "career_save.json";

        public static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        public static bool HasSave()
        {
            return File.Exists(SavePath);
        }

        public static bool TrySave(CareerSaveData data, out string error)
        {
            error = null;

            if(data == null)
            {
                error = "Save data does not exists";
                return false;
            }

            string temporaryPath = SavePath + ".tmp";

            try
            {
                string json = JsonUtility.ToJson(data, true);

                File.WriteAllText(temporaryPath, json);

                if(File.Exists(SavePath))
                {
                    File.Copy(temporaryPath, SavePath, true);
                    File.Delete(temporaryPath);
                }
                else
                {
                    File.Move(temporaryPath, SavePath);
                }

                return true;
            }
            catch(Exception exception)
            {
                error = exception.Message;

                try
                {
                    if(File.Exists(temporaryPath))
                    {
                        File.Delete(temporaryPath);
                    }
                }
                catch
                {

                }

                return false;
            }
        }

        public static bool TryLoad(
            out CareerSaveData data,
            out string error
        )
        {
            data = null;
            error = null;

            if(!File.Exists(SavePath))
            {
                error = "Saving not found";
                return false;
            }

            try
            {
                string json = File.ReadAllText(SavePath);

                Debug.Log($"Save JSON:\n{json}");
                data = JsonUtility.FromJson<CareerSaveData>(json);

                if(data == null ||
                   data.SaveVersion != 1 ||
                   data.Car == null ||
                   data.Driver == null)
                {
                    data = null;
                    error = "Saving file is damaged or have unknown version";
                    return false;
                }

                return true;
            }
            catch(Exception exception)
            {
                data = null;
                error = exception.Message;
                return false;
            }
        }

        public static bool TryDeleteSave(out string error)
        {
            error = null;

            try
            {
                if(File.Exists(SavePath))
                {
                    File.Delete(SavePath);
                }

                return true;
            }
            catch(Exception exception)
            {
                error = exception.Message;
                return false;
            }
        }

        public static bool TrySaveCareer(
            System.Collections.Generic.List<RacingProject.Domain.Team> teams,
            int completedRaces,
            out string error)
        {
            error = null;

            string temporaryPath = SavePath + ".tmp";
            string backupPath = SavePath + ".bak";

            try
            {
                FullCareerSaveData data =
                    CareerSaveMapper.CreateFullCareerSaveData(
                        teams,
                        completedRaces
                    );

                string json = JsonUtility.ToJson(data, true);

                File.WriteAllText(temporaryPath, json);

                string verificationJson = File.ReadAllText(temporaryPath);

                FullCareerSaveData verification =
                    JsonUtility.FromJson<FullCareerSaveData>(verificationJson);

                if (verification == null ||
                    verification.SaveVersion != 1 ||
                    verification.CompletedRaces != completedRaces ||
                    verification.Teams == null ||
                    verification.Teams.Count != teams.Count)
                {
                    throw new InvalidOperationException(
                        "Temporary career save verification failed."
                    );
                }

                if (File.Exists(SavePath))
                {
                    File.Replace(temporaryPath, SavePath, backupPath);
                }
                else
                {
                    File.Move(temporaryPath, SavePath);
                }

                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                Debug.Log($"Save exists: {File.Exists(SavePath)}");
                return true;
            }
            catch (Exception exception)
            {
                error = exception.Message;

                try
                {
                    if (!File.Exists(SavePath) && File.Exists(backupPath))
                    {
                        File.Copy(backupPath, SavePath);
                    }
                }
                catch (Exception restoreException)
                {
                    Debug.LogError(
                        $"Failed to restore backup: {restoreException.Message}"
                    );
                }

                try
                {
                    if (File.Exists(temporaryPath))
                    {
                        File.Delete(temporaryPath);
                    }
                }
                catch (Exception cleanupException)
                {
                    Debug.LogWarning(
                        $"Failed to delete temporary save: {cleanupException.Message}"
                    );
                }

                return false;
            }
        }

        public static bool TryLoadCareer(
            out FullCareerSaveData data,
            out System.Collections.Generic.List<RacingProject.Domain.Team> teams,
            out string error
        )
        {
            data = null;
            teams = null;

            if(!File.Exists(SavePath))
            {
                error = "Save file not found";
                return false;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                data = JsonUtility.FromJson<FullCareerSaveData>(json);

                if(data == null ||
                   data.SaveVersion != 1 ||
                   data.CompletedRaces < 0 ||
                   data.Teams == null ||
                   data.Teams.Count == 0)
                {
                    data = null;
                    error = "Career save is invalid";
                    return false;
                }

                teams = CareerSaveMapper.RestoreTeams(data);

                if(teams == null || teams.Count != data.Teams.Count)
                {
                    data = null;
                    teams = null;
                    error = "Career teams could not be restored";
                    return false;
                }

                if(teams.Exists(team =>
                   team == null ||
                   team.Car == null ||
                   team.Car.Tire == null ||
                   team.Driver == null ||
                   team.Strategy == null ||
                   team.Defence == null))
                {
                    data = null;
                    teams = null;
                    error = "Career save contains invalid team data";
                }
                
                error = null;
                return true;
            }
            catch(System.Exception exception)
            {
                data = null;
                teams = null;
                error = exception.Message;
                return false;
            }
        }
    } 
}