using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using BotCore.Execution;
using BotCore.Permission;
using BotCore.Util.Symbol;

namespace UnitatoBot.Execution {

    internal class PermissionExecutor : IInitializable, IConditionalExecutonHandler {

        // IInitializable

        public void Initialize(ExecutionManager manager) {
            RuntimeHelpers.RunClassConstructor(typeof(Permissions).TypeHandle);
        }

        // IExecutionHandler

        public string GetDescription() {
            return "Permission management. Use with argument 'users' to print out list of users in given groups, use with 'list' to get list of groups and what they can do. Create group: 'create [group] [permission...](multiple allowed)'; Destory group: 'destory [group]'; Add permission to group: 'allow [group] [permission...](multiple allowed)'; Remove permission from group: 'deny [group] [permission...](multiple allowed)'; Add user to group: 'put [user] [group]'; Remove user from group: 'take [user] [group]'; Realod from reload: 'reload'";
        }

        public bool CanExecute(ExecutionContext context) {
            return context.HasArguments;
        }

        public bool Execute(ExecutionContext context) {
            switch(context.Args[0]) {
                case "users":
                    context.ResponseBuilder
                        .Text(Emoji.Key)
                        .Username()
                        .Text("this is list of users in permission groups:")
                        .TableStart(20, "Group", "User");

                    foreach(PermissionGroup group in Permissions.Groups) {
                        AddGroupData(context, group, group.Members);
                    }

                    context.ResponseBuilder
                        .TableEnd()
                        .Send();
                    return true;

                case "list":
                    context.ResponseBuilder
                        .Text(Emoji.Key)
                        .Username()
                        .Text("this is list of permission for groups:")
                        .TableStart(20, "Group", "Permission");

                    foreach(PermissionGroup group in Permissions.Groups) {
                        AddGroupData(context, group, group.Permissions);
                    }

                    context.ResponseBuilder
                        .TableEnd()
                        .Send();
                    return true;

                case "create":
                    if(context.Args.Length > 1 && Permissions.Can(context, Permissions.PermissionCreate)) {
                        string[] perms = context.Args.Skip(2).ToArray();
                        if(Permissions.CreateGroup(context.Args[1], perms) != null) {
                            context.ResponseBuilder
                                .Text(Emoji.Key)
                                .Username()
                                .Text("created new permission group")
                                .Block(context.Args[1]);

                            if(perms.Length > 0) {
                                context.ResponseBuilder.Text("with permisisons");
                                foreach(string perm in perms) {
                                    context.ResponseBuilder.Block(perm);
                                }
                            }

                            context.ResponseBuilder.Send();
                            return true;
                        }
                    }
                    break;

                case "destroy":
                    if(context.Args.Length == 2 && Permissions.Can(context, Permissions.PermissionDestroy)) {
                        if(Permissions.DestoryGroup(context.Args[1])) {
                            context.ResponseBuilder
                                .Text(Emoji.Key)
                                .Username()
                                .Text("destoryed permission group")
                                .Block(context.Args[1])
                                .Send();

                            return true;
                        }
                    }
                    break;

                case "allow":
                    if(context.Args.Length > 2 && Permissions.Can(context, Permissions.PermissionAllow)) {
                        string[] perms = context.Args.Skip(2).ToArray();
                        if(Permissions.Allow(context.Args[1], perms).Contains(true)) {
                            context.ResponseBuilder
                                .Text(Emoji.Unlock)
                                .Username()
                                .Text("added permisison{0}", perms.Length > 1 ? "s" : string.Empty);

                            foreach(string perm in perms) {
                                context.ResponseBuilder.Block(perm);
                            }

                            context.ResponseBuilder
                                .Text("to group")
                                .Block(context.Args[1])
                                .Send();
                            return true;
                        }
                    }
                    break;

                case "deny":
                    if(context.Args.Length > 2 && Permissions.Can(context, Permissions.PermissionDeny)) {
                        string[] perms = context.Args.Skip(2).ToArray();
                        if(Permissions.Deny(context.Args[1], perms).Contains(true)) {
                            context.ResponseBuilder
                                .Text(Emoji.Lock)
                                .Username()
                                .Text("removed permisison{0}", perms.Length > 1 ? "s" : string.Empty);
 
                            foreach(string perm in perms) {
                                context.ResponseBuilder.Block(perm);
                            }

                            context.ResponseBuilder
                                .Text("from group")
                                .Block(context.Args[1])
                                .Send();
                            return true;
                        }
                    }
                    break;

                case "put":
                    if(context.Args.Length == 3 && Permissions.Can(context, Permissions.PermissionPut)) {
                        if(Permissions.Put(context.Args[1], context.Args[2])) {
                            context.ResponseBuilder
                                .Text(Emoji.Inbox)
                                .Username()
                                .Text("put user")
                                .Block(context.Args[1])
                                .Text("in permisison group")
                                .Block(context.Args[2])
                                .Send();

                            return true;
                        }
                    }
                    break;

                case "take":
                    if(context.Args.Length == 3 && Permissions.Can(context, Permissions.PermissionTake)) {
                        if(Permissions.Take(context.Args[1], context.Args[2])) {
                            context.ResponseBuilder
                                .Text(Emoji.Outbox)
                                .Username()
                                .Text("removed user")
                                .Block(context.Args[1])
                                .Text("from permisison group")
                                .Block(context.Args[2])
                                .Send();

                            return true;
                        }
                    }
                    break;

                case "reload":
                    if(context.Args.Length == 1 && Permissions.Can(context, Permissions.PermissionReload)) {
                        Permissions.Load();

                        context.ResponseBuilder
                            .Text(Emoji.Key)
                            .Username()
                            .Text("reloaded permissions from files")
                            .Send();

                        return true;
                    }
                    break;
            }

            return false;
        }

        // Util

        private void AddGroupData(ExecutionContext context, PermissionGroup group, IEnumerable enumerable) {
            if(group.MembersCount > 0) {
                bool tableStart = false;
                foreach(string user in enumerable) {
                    if(!tableStart) {
                        context.ResponseBuilder.TableRow(group.Name, user);
                        tableStart = true;
                    } else {
                        context.ResponseBuilder.TableRow(string.Empty, user);
                    }
                }
            } else {
                context.ResponseBuilder.TableRow(group.Name, string.Empty);
            }

            context.ResponseBuilder.TableSpacer();
        }

    }

}
