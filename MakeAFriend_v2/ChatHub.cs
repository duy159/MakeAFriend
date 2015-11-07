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

        private static ConcurrentDictionary<string, Room>[] rooms = new ConcurrentDictionary<string, Room>[6] {
            roomsMovies, roomsTravel, roomsGames, roomsMusic, roomsAnimals, roomsSports
        };

        private static int roomsType = -1;

        public void Send(string name, string message)
        {
            //Clients.All.broadcastMessage(name, message);
            Clients.Client(Context.ConnectionId).gettopic();

            Room currentRoom = null;
            foreach (KeyValuePair<string, Room> existingRoom in rooms[roomsType])
            {
                foreach (User u in existingRoom.Value.users) {
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
            }
        }

        public override Task OnConnected()
        {
            roomsType = -1;
            Clients.Client(Context.ConnectionId).gettopic();

            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer((obj) =>
            {
              
                CreateUser();

                Room currentRoom = null;
                foreach (KeyValuePair<string, Room> existingRoom in rooms[roomsType])
                {
                    foreach (User u in existingRoom.Value.users)
                    {
                        if (Context.ConnectionId.Equals(u.connectionId))
                        {
                            if (existingRoom.Key.Equals(u.roomId))
                            {
                                currentRoom = existingRoom.Value;
                                break;
                            }
                        }
                    }
                }
                foreach (User u in currentRoom.users)
                {
                    Clients.Client(u.connectionId).connectionMessage(" has joined the room.");
                }


                timer.Dispose();
            }, null, 1000, System.Threading.Timeout.Infinite);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Clients.Client(Context.ConnectionId).gettopic();

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
                Clients.Client(u.connectionId).connectionMessage(" has left the room.");
            }
            return base.OnDisconnected(stopCalled);
        }

        private void CreateUser()
        {
            Debug.WriteLine("New user created");

            User user = new User();
            user.name = Context.User.Identity.Name;
            user.connectionId = Context.ConnectionId;
            FindRoom(user);
        }

        private void FindRoom(User user)
        {
            Debug.WriteLine("Finding room");


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
                    break;
                }
            }

            if (!roomFound)
            {
                Debug.WriteLine("Room not found, creating new room");

                Room r = new Room();
                string roomId = Guid.NewGuid().ToString();
                user.roomId = roomId;
                r.users.Add(user);
                rooms[roomsType].TryAdd(roomId, r);
            }
        }
    }

    public class User
    {
        public string connectionId { get; set; }
        public string name { get; set; }
        public string roomId { get; set; }
    }

    public class Room
    {
        public ArrayList users = new ArrayList();
    }
}