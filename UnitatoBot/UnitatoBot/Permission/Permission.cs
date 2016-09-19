using System.Collections.Generic;
using System.IO;

namespace UnitatoBot.Permission {

    internal static class Permissions {

        private static LinkedList<PermissionGroup> Groups = new LinkedList<PermissionGroup>();

        public static string All { private set; get; } = "*";
        public static string Reboot { private set; get; } = "reboot";

        public static void Load() {
            Logger.Log("Loading Permission... ");
            Logger.SectionStart();

            Groups.Clear();

            DirectoryInfo dirPerm = new DirectoryInfo("permission");
            if(dirPerm.Exists) {
                foreach(FileInfo file in dirPerm.GetFiles("group*.json", SearchOption.TopDirectoryOnly)) {
                    PermissionGroup group = PermissionGroup.FromFile(file);

                    if(group != null) {
                        Groups.AddLast(group);
                        Logger.Info("Loaded group {0} with {1} permission entr{2} and {3} user{4}", group.Name, group.PermissionCount, group.PermissionCount > 1 ? "ies" : "y", group.MembersCount, group.MembersCount > 1 ? "s" : string.Empty);
                    }
                }
                Logger.Info("Loaded {0} permission group{1}!", Groups.Count, Groups.Count > 1 ? "s" : string.Empty);
            } else {
                Directory.CreateDirectory("permission");
                Logger.Warn("Permission directory is missing generating one...");

                Groups.AddLast(new PermissionGroup("Admin", All));
                Logger.Info("Generated Admin permission group...");
            }

            Logger.SectionEnd();
            Logger.Log("Permission loaded.");
        }

        public static bool Has(string user, string permission) {
            foreach(PermissionGroup group in Groups) {
                if(group.HasPermission(user, permission))
                    return true;
            }

            return false;
        }

    }

}
