using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class SignalRequester : RunAbleThread
{
    public override void Run()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use

        using (RequestSocket client = new RequestSocket())
        {
            client.Connect("tcp://localhost:5555");

			string signal = "None";
			bool gotMessage = false;

			//Debug.Log("----Sending Signal----");
			client.SendFrame(message);

			while (Running)
			{
				gotMessage = client.TryReceiveFrameString(out signal); // this returns true if it's successful
				if (gotMessage) break;
			}

			if (gotMessage)
			{
				//Debug.Log("----Recived Signal: " + signal);
				EmotionInput.activeEmotion = signal;
				Running = false;
			}
        }
        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use
    }

}