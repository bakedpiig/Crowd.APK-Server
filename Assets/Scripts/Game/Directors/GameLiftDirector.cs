using Aws.GameLift;
using Aws.GameLift.Server;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Crowd.Game
{
    public class GameLiftDirector: SubDirector
    {
        private Dictionary<int, string> playerSessions = new Dictionary<int, string>();
        public bool Initialized { get; private set; }
        private int port = 1935;

        private void Awake()
        {
            Debug.Log("GameLift Server Opened!");
            Debug.Log($"SDK Version: {GameLiftServerAPI.GetSdkVersion().Result}");

            var initOutCome = GameLiftServerAPI.InitSDK();
            if (initOutCome.Success)
            {
                Debug.Log("Success to initialize SDK");

                var processParameters = new ProcessParameters(
                    // onStartGameSession
                    (gameSession) =>
                    {
                        try
                        {
                            var outcome = GameLiftServerAPI.ActivateGameSession();
                            if (outcome.Success)
                                Debug.Log("Game session activated");
                            else
                                Debug.Log($"Game session activation failed. ActivateGameSession() returned {outcome.Error.ToString()}");
                        }
                        catch (Exception e)
                        {
                            Debug.Log($"Game session activation failed. ActivateGameSession() returned {e.Message}");
                        }
                    },
                    // onProcessTerminate
                    () =>
                    {
                        Debug.Log("GameLift process termination requested. BYE BYE");
                    },
                    // onHealthCheck
                    () =>
                    {
                        Debug.Log("GameLift health requested");
                        return true;
                    },
                    port,
                    new LogParameters(new List<string>()
                    {
                        "../GameLift_Data/output_log.txt"
                    }));

                var processReadyoutcome = GameLiftServerAPI.ProcessReady(processParameters);
                if (processReadyoutcome.Success)
                {
                    Debug.Log("ProcessReady success");
                    Initialized = true;
                }
                else
                    Debug.Log("ProcessReady failed.");
            }
            else
            {
                Debug.Log("Server is not in fleet");
            }
        }

        public bool ConnectPlayer(int playerIdx, string playerSessionId)
        {
            try
            {
                var outcome = GameLiftServerAPI.AcceptPlayerSession(playerSessionId);
                if (outcome.Success)
                {
                    Debug.Log($"Player session validated, SessionId: {playerSessionId}");
                }
                else
                    Debug.Log($"Player session rejected. {outcome.Error.ToString()}");

                playerSessions.Add(playerIdx, playerSessionId);
                return outcome.Success;
            }
            catch(Exception e)
            {
                Debug.Log($"Player session rejected. {e.Message}");
                return false;
            }
        }

        public void DisconnectPlayer(int playerIdx)
        {
            try
            {
                string playerSessionId = playerSessions[playerIdx];
                try
                {
                    var outcome = GameLiftServerAPI.RemovePlayerSession(playerSessionId);
                    if (outcome.Success)
                        Debug.Log($"Player session removed, Removed session ID: {playerSessionId}");
                    else
                        Debug.Log($"Player session remove failed. {outcome.Error.ToString()}");
                }
                catch(Exception e)
                {
                    Debug.Log($"Player session remove failed. {e.Message}");
                }
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log(":( INVALID PLAYER SESSION. Exception " + Environment.NewLine + e.Message);
                throw;
            }
        }
    }
}