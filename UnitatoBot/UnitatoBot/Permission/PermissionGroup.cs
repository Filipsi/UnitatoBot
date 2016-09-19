using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace UnitatoBot.Permission {

    [JsonObject(MemberSerialization.OptIn)]
    internal class PermissionGroup {

        [JsonProperty]
        public string   Name            { private set; get; }

        public int      PermissionCount { get { return Permissions.Count; } }
        public int      MembersCount    { get { return Members.Count; } }

        [JsonProperty]
        private LinkedList<string>  Permissions;

        [JsonProperty]
        private List<string>        Members;

        public PermissionGroup() {
            //NO-OP
        }

        public PermissionGroup(string name, params string[] permissions) {
            Name = name;
            Members = new List<string>();
            Permissions = new LinkedList<string>(permissions);

            ToFile();
        }

        public bool AddMember(string memeber, bool save = true) {
            if(!Members.Contains(memeber)) {
                Members.Add(memeber);
                if(save) ToFile();
                return true;
            }

            return false;
        }

        public bool AddPermission(string permission, bool save = true) {
            if(!Permissions.Contains(permission)) {
                Permissions.AddLast(permission);
                if(save) ToFile();
                return true;
            }

            return false;
        }

        public bool HasPermission(string user, string permission) {
            return Members.Contains(user) && (Permissions.Contains(permission) || Permissions.Contains(Permission.Permissions.All));
        }

        public void ToFile() {
            using(StreamWriter writer = File.CreateText(Path.Combine("permission", string.Format("group{0}.json", Name)))) {
                writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }

        public static PermissionGroup FromFile(FileInfo file) {
            if(!file.Exists) {
                Logger.Warn("File {0} does not exists, PermissionGroup wasn't constucted.", file.Name);
                return null;
            }

            string data = "{}";
            using(StreamReader reader = file.OpenText()) {
                data = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<PermissionGroup>(data);
        }

    }

}
