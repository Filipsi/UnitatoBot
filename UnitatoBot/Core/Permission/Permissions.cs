using System.Collections.Generic;
using System.IO;
using System.Linq;
using BotCore.Command;
using BotCore.Util;

namespace BotCore.Permission {

    public static class Permissions {

        public static string All                { private set; get; } = "*";
        public static string Reboot             { private set; get; } = "reboot";
        public static string PermissionCreate   { private set; get; } = "perm.create";
        public static string PermissionDestroy  { private set; get; } = "perm.destroy";
        public static string PermissionAllow    { private set; get; } = "perm.allow";
        public static string PermissionDeny     { private set; get; } = "perm.deny";
        public static string PermissionPut      { private set; get; } = "perm.put";
        public static string PermissionTake     { private set; get; } = "perm.take";
        public static string PermissionReload   { private set; get; } = "perm.reload";
        public static string FaggotPurify       { private set; get; } = "faggot.purify";
        public static string FaggotClean        { private set; get; } = "faggot.clean";

        public static WrappedEnumerable<PermissionGroup> Groups { private set; get; } = new WrappedEnumerable<PermissionGroup>(GroupList);

        private static LinkedList<PermissionGroup> GroupList = new LinkedList<PermissionGroup>();

        static Permissions() {
            Load();
        }

        public static void Load() {
            Logger.Log("Loading Permission... ");
            Logger.SectionStart();

            GroupList.Clear();

            DirectoryInfo dirPerm = new DirectoryInfo("permission");
            if(dirPerm.Exists) {
                foreach(FileInfo file in dirPerm.GetFiles("group*.json", SearchOption.TopDirectoryOnly)) {
                    PermissionGroup group = PermissionGroup.FromFile(file);

                    if(group != null) {
                        GroupList.AddLast(group);
                        Logger.Info("Loaded group {0} with {1} permission entr{2} and {3} user{4}", group.Name, group.PermissionCount, group.PermissionCount > 1 ? "ies" : "y", group.MembersCount, group.MembersCount > 1 ? "s" : string.Empty);
                    }
                }
                Logger.Info("Loaded {0} permission group{1}!", GroupList.Count, GroupList.Count > 1 ? "s" : string.Empty);
            } else {
                Directory.CreateDirectory("permission");
                Logger.Warn("Permission directory is missing generating one...");
            }

            if(!GroupList.Any(x => x.Name.Equals("Admin"))) {
                Logger.Info("Admin permission group is mising, generating default one ...");
                GroupList.AddLast(new PermissionGroup("Admin", All));
            }

            Logger.SectionEnd();
            Logger.Log("Permission loaded.");
        }

        public static bool Can(string user, string permission) {
            foreach(PermissionGroup group in GroupList) {
                if(group.IsPermited(user, permission))
                    return true;
            }

            return false;
        }

        public static bool Can(CommandContext context, string permission) {
            return Can(context.ServiceMessage.Sender, permission);
        }

        public static bool Put(string user, string group) {
            PermissionGroup g = GroupList.SingleOrDefault(x => x.Name.Equals(group));
            if(g != null && !g.HasMember(user)) {
                g.AddMember(user);
                return true;
            }

            return false;
        }

        public static bool Take(string user, string group) {
            PermissionGroup g = GroupList.SingleOrDefault(x => x.Name.Equals(group));
            if(g != null && g.HasMember(user)) {
                g.RemoveMember(user);
                return true;
            }

            return false;
        }

        public static bool[] Allow(string group, params string[] permissions) {
            PermissionGroup g = GroupList.SingleOrDefault(x => x.Name.Equals(group));

            bool[] results = new bool[permissions.Length];
            if(g != null) {
                for(short i = 0; i < permissions.Length; i++) {
                    if(!g.HasPermission(permissions[i]))
                        results[i] = g.AddPermission(permissions[i]);
                }
            }

            return results;
        }

        public static bool[] Deny(string group, params string[] permissions) {
            PermissionGroup g = GroupList.SingleOrDefault(x => x.Name.Equals(group));

            bool[] results = new bool[permissions.Length];
            if(g != null) {
                for(short i = 0; i < permissions.Length; i++) {
                    if(g.HasPermission(permissions[i]))
                        results[i] = g.RemovePermission(permissions[i]);
                }
            }

            return results;
        }

        public static PermissionGroup CreateGroup(string name, params string[] permissions) {
            if(!GroupList.Any(x => x.Name.Equals(name))) {
                GroupList.AddLast(new PermissionGroup(name, permissions));
                return GroupList.Last.Value;
            }

            return null;
        }

        public static bool DestoryGroup(string name) {
            PermissionGroup g = GroupList.SingleOrDefault(x => x.Name.Equals(name));

            if(g != null) {
                GroupList.Remove(g);
                g.Destory();
                return true;
            }

            return false;
        }

    }

}
