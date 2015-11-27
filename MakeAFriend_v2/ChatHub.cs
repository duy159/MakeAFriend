using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections;
using System.Diagnostics;

namespace MakeAFriend_v2
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, Room> roomsMovies = new ConcurrentDictionary<string, Room>();
        private static ConcurrentDictionary<string, Room> roomsTravel = new ConcurrentDictionary<string, Room>();
        private static ConcurrentDictionary<string, Room> roomsGames = new ConcurrentDictionary<string, Room>();
        private static ConcurrentDictionary<string, Room> roomsMusic = new ConcurrentDictionary<string, Room>();
        private static ConcurrentDictionary<string, Room> roomsAnimals = new ConcurrentDictionary<string, Room>();
        private static ConcurrentDictionary<string, Room> roomsSports = new ConcurrentDictionary<string, Room>();
        private static ConcurrentDictionary<string, Room> roomsFriend = new ConcurrentDictionary<string, Room>();

        private static ConcurrentDictionary<string, Room>[] rooms = new ConcurrentDictionary<string, Room>[7] {
            roomsMovies, roomsTravel, roomsGames, roomsMusic, roomsAnimals, roomsSports, roomsFriend
        };

        private static ConcurrentDictionary<string, string> userInfos = new ConcurrentDictionary<string, string>();

        private static int roomsType = -1;
        private static string friendName = "";
        private static string friendIp = "";

        public void Send(string name, string message)
        {
            FindRoomType();

            if (roomsType == -1)
            {
                roomsType = 6;
            }

            Room currentRoom = null;
            foreach (KeyValuePair<string, Room> existingRoom in rooms[roomsType])
            {
                foreach (User u in existingRoom.Value.users)
                {
                    if (Context.ConnectionId.Equals(u.connectionId) && existingRoom.Key.Equals(u.roomId))
                    {
                        currentRoom = existingRoom.Value;
                        break;
                    }
                }
            }

            foreach (User u in currentRoom.users)
            {
                Clients.Client(u.connectionId).broadcastMessage(name, message);
            }
        }

        private void FindRoomType()
        {
            int i = 0;
            foreach (ConcurrentDictionary<string, Room> topicRooms in rooms)
            {
                foreach (KeyValuePair<string, Room> existingRoom in topicRooms)
                {
                    foreach (User u in existingRoom.Value.users)
                    {
                        if (Context.ConnectionId.Equals(u.connectionId) && existingRoom.Key.Equals(u.roomId))
                        {
                            roomsType = i;
                            break;
                        }
                    }
                }
                i++;
            }
        }

        public void GetOtherUser()
        {
            string userid = "";

            Room currentRoom = null;
            foreach (KeyValuePair<string, Room> existingRoom in rooms[roomsType])
            {
                foreach (User u in existingRoom.Value.users)
                {
                    if (Context.ConnectionId.Equals(u.connectionId) && existingRoom.Key.Equals(u.roomId))
                    {
                        currentRoom = existingRoom.Value;
                        break;
                    }
                }
            }
            string connectionid = "";
            foreach (User u in currentRoom.users)
            {
                if (u.name != Context.User.Identity.Name)
                {
                    connectionid = u.connectionId;
                    userid = u.name;
                    break;
                }
            }

            Clients.Client(Context.ConnectionId).getotheruser(userid, connectionid);
        }

        public void ReturnValue(string returnvalue)
        {
            Debug.WriteLine(returnvalue);
            switch (returnvalue)
            {
                case "Movies":
                    roomsType = 0;
                    break;
                case "Travel":
                    roomsType = 1;
                    break;
                case "Games":
                    roomsType = 2;
                    break;
                case "Music":
                    roomsType = 3;
                    break;
                case "Animals":
                    roomsType = 4;
                    break;
                case "Sports":
                    roomsType = 5;
                    break;
                case "Friend":
                    roomsType = 6;
                    break;
                case "FriendJoining":
                    roomsType = -1;
                    break;
                default:
                    roomsType = -2;
                    break;
            }
        }

        public void ReturnFriend(string name, string ip)
        {
            friendName = name;
            friendIp = ip;
        }

        public override Task OnConnected()
        {
            Debug.WriteLine("Client's connection id: " + Context.ConnectionId);
            //userInfos.TryAdd(Context.User.Identity.Name, Context.ConnectionId);
            userInfos.AddOrUpdate(Context.User.Identity.Name, Context.ConnectionId, (key, oldValue) => Context.ConnectionId);

            Clients.Client(Context.ConnectionId).gettopic();

            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer((obj) =>
            {

                if (roomsType != 6 && roomsType != -1 && roomsType != -2)
                {
                    //foreach (KeyValuePair<string, string> existingUsers in userInfos)
                    //{
                    //    if (existingUsers.Key == Context.User.Identity.Name)
                    //    {
                    //        string garbage;
                    //        userInfos.TryRemove(existingUsers.Key, out garbage);
                    //        break;
                    //    }
                    //}

                    CreateUser();

                    Room currentRoom = null;
                    foreach (KeyValuePair<string, Room> existingRoom in rooms[roomsType])
                    {
                        foreach (User u in existingRoom.Value.users)
                        {
                            if (Context.ConnectionId.Equals(u.connectionId) && existingRoom.Key.Equals(u.roomId))
                            {
                                currentRoom = existingRoom.Value;
                                break;
                            }
                        }
                    }

                    if (currentRoom.users.Count == 2)
                    {
                        foreach (User u in currentRoom.users)
                        {
                            Clients.Client(u.connectionId).foundMatch();
                            Clients.Client(u.connectionId).connectionMessage(" have joined the room.");
                        }
                    }
                } else if (roomsType == 6 && roomsType != -2 && roomsType != -1)
                {
                    Clients.Client(Context.ConnectionId).getfriendinfo();

                    System.Threading.Timer timer2 = null;
                    timer2 = new System.Threading.Timer((obj2) =>
                    {
                        CreateUser();

                        Room currentRoom = null;
                        foreach (KeyValuePair<string, Room> existingRoom in rooms[roomsType])
                        {
                            foreach (User u in existingRoom.Value.users)
                            {
                                if (Context.ConnectionId.Equals(u.connectionId) && existingRoom.Key.Equals(u.roomId))
                                {
                                    currentRoom = existingRoom.Value;
                                    break;
                                }
                            }
                        }

                        foreach (User u in currentRoom.users)
                        {
                            Clients.Client(Context.ConnectionId).foundMatch();
                            if (u.connectionId != Context.ConnectionId)
                            {
                                Debug.WriteLine("Found friend");
                                Clients.Client(u.connectionId).roominvitation(Context.User.Identity.Name, Context.ConnectionId);
                                //Clients.All.roominvitation();
                            }
                        }

                        timer2.Dispose();
                    }, null, 4000, System.Threading.Timeout.Infinite);
                } else if (roomsType == -1)
                {
                    roomsType = 6;

                    Room currentRoom = null;
                    foreach (KeyValuePair<string, Room> existingRoom in rooms[roomsType])
                    {
                        foreach (User u in existingRoom.Value.users)
                        {
                            if (Context.User.Identity.Name.Equals(u.name) && existingRoom.Key.Equals(u.roomId))
                            {
                                currentRoom = existingRoom.Value;
                                break;
                            }
                        }
                    }

                    foreach (User u in currentRoom.users)
                    {
                        if (u.name == Context.User.Identity.Name)
                        {
                            u.connectionId = Context.ConnectionId;
                            Clients.Client(u.connectionId).foundMatch();
                        }
                    }
                }


                timer.Dispose();
            }, null, 1500, System.Threading.Timeout.Infinite);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Clients.Client(Context.ConnectionId).gettopic();

            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer((obj) =>
            {
                if (roomsType != -1 && roomsType != -2)
                {
                    bool roomFound = false;
                    Room currentRoom = null;
                    foreach (KeyValuePair<string, Room> existingRoom in rooms[roomsType])
                    {
                        foreach (User u in existingRoom.Value.users)
                        {
                            if (Context.ConnectionId.Equals(u.connectionId) && existingRoom.Key.Equals(u.roomId))
                            {
                                currentRoom = existingRoom.Value;
                                roomFound = true;
                                break;
                            }
                        }
                    }
                    if (roomFound)
                    {
                        foreach (User u in currentRoom.users)
                        {
                            if (u.connectionId != Context.ConnectionId && currentRoom.users.Count == 2)
                            {
                                Clients.Client(u.connectionId).connectionMessage(" have left the room.");
                            }

                            //if (currentRoom.users.Count < 2 && u.connectionId == Context.ConnectionId)
                            //{
                            //    currentRoom.users.Remove(u);
                            //    break;
                            //}

                            if (!currentRoom.full)
                            {
                                Debug.WriteLine("Canceling match");
                                currentRoom.users.Remove(u);
                                break;
                            }
                        }
                    }
                }
                //if (roomsType == 6 || roomsType == -1)
                //{
                //    foreach (KeyValuePair<string, string> existingUsers in userInfos)
                //    {
                //        if (existingUsers.Key == Context.User.Identity.Name)
                //        {
                //            string garbage;
                //            userInfos.TryRemove(existingUsers.Key, out garbage);
                //            break;
                //        }
                //    }
                //}
                timer.Dispose();
            }, null, 1500, System.Threading.Timeout.Infinite);

            return base.OnDisconnected(stopCalled);
        }

        private void CreateUser()
        {
            Debug.WriteLine("Creating new user");

            User user = new User();
            user.name = Context.User.Identity.Name;
            user.connectionId = Context.ConnectionId;
            user.roomType = roomsType;

            if (roomsType != 6)
            {
                FindRoom(user);
            } else if (roomsType == 6)
            {
                User friend = new User();
                friend.name = friendName;
                friend.roomType = roomsType;

                foreach (KeyValuePair<string, string> existingUsers in userInfos)
                {
                    if (existingUsers.Key == friendName)
                    {
                        Debug.WriteLine("Creating user for friend");
                        friend.connectionId = existingUsers.Value;
                        break;
                    }
                }
                Debug.WriteLine("Creating new room");
                Room r = new Room();
                string roomId = Guid.NewGuid().ToString();
                user.roomId = roomId;
                friend.roomId = roomId;
                r.users.Add(friend);
                r.users.Add(user);
                rooms[roomsType].TryAdd(roomId, r);
            }
        }

        private static void CreateRoom(User user)
        {
            Debug.WriteLine("Creating new room");
            Room r = new Room();
            string roomId = Guid.NewGuid().ToString();
            r.full = false;
            user.roomId = roomId;
            r.users.Add(user);
            rooms[roomsType].TryAdd(roomId, r);
        }

        private void FindRoom(User user)
        {
            Debug.WriteLine("Searching for room");

            Room room;
            bool roomFound = false;
            foreach (KeyValuePair<string, Room> existingRoom in rooms[roomsType])
            {
                room = existingRoom.Value;

                if(room.users.Count <= 1)
                {
                    Debug.WriteLine("Room found");

                    roomFound = true;
                    user.roomId = existingRoom.Key;
                    room.users.Add(user);
                    room.full = true;
                    break;
                }
            }

            if (!roomFound)
            {
                Debug.WriteLine("Room not found");

                //Room r = new Room();
                //string roomId = Guid.NewGuid().ToString();
                //user.roomId = roomId;
                //r.users.Add(user);
                //rooms[roomsType].TryAdd(roomId, r);
                CreateRoom(user);
            }
        }
    }

    public class User
    {
        public string connectionId { get; set; }
        public string name { get; set; }
        public string roomId { get; set; }
        public int roomType { get; set; }
    }

    public class Room
    {
        public bool full { get; set; }
        public ArrayList users = new ArrayList();
    }
}