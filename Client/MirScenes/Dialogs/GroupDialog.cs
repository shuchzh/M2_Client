﻿using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using C = ClientPackets;

namespace Client.MirScenes.Dialogs
{
    public sealed class GroupDialog : MirImageControl
    {
        public static bool AllowGroup;
        public static List<string> GroupList = new List<string>();

        //public MirImageControl TitleLabel;
        public MirButton SwitchButton, CloseButton, AddButton, DelButton,NewGroupButton;
        public MirLabel[] GroupMembers;

        public GroupDialog()
        {
            Index = 120;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Location = Center;

            GroupMembers = new MirLabel[Globals.MaxGroup];

            GroupMembers[0] = new MirLabel
            {
                AutoSize = true,
                Location = new Point(16, 33),
                Parent = this,
                NotControl = true,
            };

            for (int i = 1; i < GroupMembers.Length; i++)
            {
                GroupMembers[i] = new MirLabel
                {
                    AutoSize = true,
                    Location = new Point(((i + 1) % 2) * 100 + 16, 55 + ((i - 1) / 2) * 20),
                    Parent = this,
                    NotControl = true,
                };
            }


            #region title
            //TitleLabel = new MirImageControl
            //{
            //    Index = 5,
            //    Library = Libraries.Title,
            //    Location = new Point(18, 8),
            //    Parent = this
            //};
            #endregion
            CloseButton = new MirButton
            {
                HoverIndex = 233,//361,
                Location = new Point(260, 0),
                Library = Libraries.Prguse3,
                Parent = this,
                TakeSizeMode = UsedSize.HoverIndex,
                PressedIndex = 234,//362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            SwitchButton = new MirButton
            {
                HoverIndex = 121,
                Index = 121,
                Location = new Point(20, 18),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 122,
                Sound = SoundList.ButtonA,
            };
            SwitchButton.Click += (o, e) => Network.Enqueue(new C.SwitchGroup { AllowGroup = !AllowGroup });

            NewGroupButton = new MirButton
            {
                //HoverIndex = 124,
                //Index = 124,//133,
                Location = new Point(21, 202),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 123,
                Sound = SoundList.ButtonA,
                TakeSizeMode = UsedSize.PressedIndex,
            };
            NewGroupButton.Click +=(o,e)=> NewGroup();

            AddButton = new MirButton
            {
                //HoverIndex = 124,
                //Index = 124,//133,
                Location = new Point(96, 202),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 124,
                Sound = SoundList.ButtonA,
                TakeSizeMode=UsedSize.PressedIndex,
            };
            AddButton.Click += (o, e) => AddMember();

            DelButton = new MirButton
            {
                //HoverIndex = 125,
                //Index = 125,//136,
                Location = new Point(171, 202),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 125,
                Sound = SoundList.ButtonA,
                TakeSizeMode=UsedSize.PressedIndex,
            };
            DelButton.Click += (o, e) => DelMember();

            BeforeDraw += GroupPanel_BeforeDraw;

            GroupList.Clear();
        }

        private void NewGroup()
        {
            if (GroupList.Count > 0)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("Your group already exists.", MessageType.System);
                return;
            }
            MirInputBox inputBox = new MirInputBox("Please enter the name of the person you wish to group.");

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.AddMember { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
        }

        private void GroupPanel_BeforeDraw(object sender, EventArgs e)
        {
            #region new UI
            //if (GroupList.Count == 0)
            //{
            //    //AddButton.Index = 130;
            //    AddButton.HoverIndex = 124;
            //    AddButton.PressedIndex = 124;
            //}
            //else
            //{
            //    //AddButton.Index = 133;
            //    AddButton.HoverIndex = 124;//134;
            //    AddButton.PressedIndex = 124;// 135;
            //}
            //if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            //{
            //    AddButton.Visible = false;
            //    DelButton.Visible = false;
            //}
            //else
            //{
            //    AddButton.Visible = true;
            //    DelButton.Visible = true;
            //}
            #endregion
            if (AllowGroup)
            {
                SwitchButton.Index = 122;
                SwitchButton.HoverIndex = 122;
                SwitchButton.PressedIndex = 121;
            }
            else
            {
                SwitchButton.Index = 121;
                SwitchButton.HoverIndex = 121;
                SwitchButton.PressedIndex = 122;
            }

            for (int i = 0; i < GroupMembers.Length; i++)
                GroupMembers[i].Text = i >= GroupList.Count ? string.Empty : GroupList[i];
        }

        public void AddMember(string name)
        {
            if (GroupList.Count >= Globals.MaxGroup)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("Your group already has the maximum number of members.", MessageType.System);
                return;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("You are not the leader of your group.", MessageType.System);
                return;
            }

            Network.Enqueue(new C.AddMember { Name = name });
        }

        private void AddMember()
        {
            if (GroupList.Count >= Globals.MaxGroup)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("Your group already has the maximum number of members.", MessageType.System);
                return;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {

                GameScene.Scene.ChatDialog.ReceiveChat("You are not the leader of your group.", MessageType.System);
                return;
            }

            MirInputBox inputBox = new MirInputBox("Please enter the name of the person you wish to group.");

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.AddMember { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }
        private void DelMember()
        {
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {

                GameScene.Scene.ChatDialog.ReceiveChat("You are not the leader of your group.", MessageType.System);
                return;
            }
            if (GroupList.Count <= 0)
                return;
            MirInputBox inputBox = new MirInputBox("Please enter the name of the person you wish to group.");

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.DelMember { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }


        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
        }
        public void Show()
        {
            if (Visible) return;
            Visible = true;
        }
    }
}
