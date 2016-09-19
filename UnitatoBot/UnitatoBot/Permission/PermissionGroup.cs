using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace UnitatoBot.Permission {

    [JsonObject(MemberSerialization.OptIn)]
    internal class PermissionGroup {

        [JsonProperty(PropertyName = "Name")]
        public string Name { private set; get; }

        public int                          PermissionCount { get { return PermissionList.Count; } }
        public int                          MembersCount    { get { return MemberList.Count; } }
        public WrappedEnumerable<string>    Permissions     { private set; get; }
        public WrappedEnumerable<string>    Members         { private set; get; }

        [JsonProperty(PropertyName ="Permissions")]
        private LinkedList<string>  PermissionList;

        [JsonProperty(PropertyName ="Members")]
        private List<string>        MemberList;

        public PermissionGroup() {
            // NO-OP
        }

        public PermissionGroup(string name, params string[] permissions) {
            Name = name;
            MemberList = new List<string>();
            Members = new WrappedEnumerable<string>(MemberList);
            PermissionList = new LinkedList<string>(permissions);
            Permissions = new WrappedEnumerable<string>(PermissionList);

            ToFile();
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context) {
            Members = new WrappedEnumerable<string>(MemberList);
            Permissions = new WrappedEnumerable<string>(PermissionList);
        }

        public bool AddMember(string memeber, bool save = true) {
            if(!MemberList.Contains(memeber)) {
                MemberList.Add(memeber);
                if(save) ToFile();
                return true;
            }

            return false;
        }

        public bool RemoveMember(string memeber, bool save = true) {
            if(MemberList.Contains(memeber)) {
                MemberList.Remove(memeber);
                if(save) ToFile();
                return true;
            }

            return false;
        }

        public bool AddPermission(string permission, bool save = true) {
            if(!PermissionList.Contains(permission)) {
                PermissionList.AddLast(permission);
                if(save) ToFile();
                return true;
            }

            return false;
        }

        public bool RemovePermission(string permission, bool save = true) {
            if(PermissionList.Contains(permission)) {
                PermissionList.Remove(permission);
                if(save) ToFile();
                return true;
            }

            return false;
        }

        public bool HasMember(string user) {
            return MemberList.Contains(user);
        }

        public bool HasPermission(string permission) {
            return PermissionList.Contains(permission);
        }

        public bool IsPermited(string user, string permission) {
            return MemberList.Contains(user) && (PermissionList.Contains(permission) || PermissionList.Contains(Permission.Permissions.All));
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

        public void Destory() {
            FileInfo file = new FileInfo(Path.Combine("permission", string.Format("group{0}.json", Name)));

            if(file.Exists)
                file.Delete();
            else Logger.Warn("File {0} does not exists, PermissionGroup wasn't be destroyed", file.Name);
        }

    }

}
