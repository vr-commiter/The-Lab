using System.Collections.Generic;
using System.Threading;
using System.IO;
using System;
using TrueGearSDK;
using System.Linq;


namespace MyTrueGear
{
    public class TrueGearMod
    {
        private static TrueGearPlayer _player = null;

        private static ManualResetEvent lefthandlaszerMRE = new ManualResetEvent(false);
        private static ManualResetEvent righthandlaszerMRE = new ManualResetEvent(false);
        private static ManualResetEvent lefthandnewballoonMRE = new ManualResetEvent(false);
        private static ManualResetEvent righthandnewballoonMRE = new ManualResetEvent(false);


        public void LeftHandLaszer()
        {
            while (true)
            {
                lefthandlaszerMRE.WaitOne();
                _player.SendPlay("LeftHandLaszer");
                Thread.Sleep(300);
            }
        }

        public void RightHandLaszer()
        {
            while (true)
            {
                righthandlaszerMRE.WaitOne();
                _player.SendPlay("RightHandLaszer");
                Thread.Sleep(300);
            }
        }

        public void LeftHandNewBalloon()
        {
            while (true)
            {
                lefthandnewballoonMRE.WaitOne();
                _player.SendPlay("LeftHandNewBalloon");
                Thread.Sleep(100);
            }
        }

        public void RightHandNewBalloon()
        {
            while (true)
            {
                righthandnewballoonMRE.WaitOne();
                _player.SendPlay("RightHandNewBalloon");
                Thread.Sleep(100);
            }
        }


        public TrueGearMod() 
        {
            _player = new TrueGearPlayer("450390","TheLab");

            //_player = new TrueGearPlayer();
            //RegisterFilesFromDisk();

            _player.Start();
            new Thread(new ThreadStart(this.LeftHandLaszer)).Start();
            new Thread(new ThreadStart(this.RightHandLaszer)).Start();
            new Thread(new ThreadStart(this.LeftHandNewBalloon)).Start();
            new Thread(new ThreadStart(this.RightHandNewBalloon)).Start();
        }

        //private void RegisterFilesFromDisk()
        //{
        //    FileInfo[] files = new DirectoryInfo(".//BepInEx//plugins//TrueGear").GetFiles("*.asset_json", SearchOption.AllDirectories);

        //    for (int i = 0; i < files.Length; i++)
        //    {
        //        string name = files[i].Name;
        //        string fullName = files[i].FullName;
        //        if (name == "." || name == "..")
        //        {
        //            continue;
        //        }
        //        string jsonStr = File.ReadAllText(fullName);
        //        JSONNode jSONNode = JSON.Parse(jsonStr);
        //        EffectObject _curAssetObj = EffectObject.ToObject(jSONNode.AsObject);
        //        string uuidName = Path.GetFileNameWithoutExtension(fullName);
        //        _curAssetObj.uuid = uuidName;
        //        _curAssetObj.name = uuidName;
        //        _player.SetupRegister(uuidName, jsonStr);
        //    }
        //}


        public void Play(string Event)
        { 
            _player.SendPlay(Event);
        }

        public void StartLeftHandLaszer()
        {
            lefthandlaszerMRE.Set();
        }

        public void StopLeftHandLaszer()
        {
            lefthandlaszerMRE.Reset();
        }

        public void StartRightHandLaszer() 
        {
            righthandlaszerMRE.Set();
        }

        public void StopRightHandLaszer()
        {
            righthandlaszerMRE.Reset();
        }

        public void StartLeftHandNewBalloon()
        {
            lefthandnewballoonMRE.Set();
        }

        public void StopLeftHandNewBalloon()
        {
            lefthandnewballoonMRE.Reset();
        }

        public void StartRightHandNewBalloon()
        {
            righthandnewballoonMRE.Set();
        }

        public void StopRightHandNewBalloon()
        {
            righthandnewballoonMRE.Reset();
        }


    }
}
