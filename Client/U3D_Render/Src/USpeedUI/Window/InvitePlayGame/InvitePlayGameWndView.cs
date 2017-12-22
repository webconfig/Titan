﻿/*******************************************************************
** 文件名:	SNSInviteBuddyWndView.cs
** 版  权:	(C) 深圳冰川网络技术有限公司 2014 - Speed
** 创建人:	李有红
** 日  期:	2017/06/02
** 版  本:	1.0
** 描  述:	邀请一起游戏界面
** 应  用:  
 *****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using U3D_Render.Common;
using DataCenter;
using DG.Tweening;
using USpeedUI.UWidgets;
using UIWidgets;
using GameLogic;
using ASpeedGame.Data.ShieldWorld;
using USpeedUI.UExtensions;
using UnityEngine.EventSystems;

namespace USpeedUI.InvitePlayGame
{
    // 邀请一起游戏分组类型
    public enum InvitePlayGameGroup
    {
        emBuddyGroup = 1,
        emKinGroup,
        emNearbyGroup
    }

    // 邀请一起游戏玩家信息
    public struct InvitePlayGamePlayerInfo
    {
        public int nUserID;     

        public string ActorName;

        public int nGroupID;

        public int nHeadIconID;

        public int nGameState;

        public int nSex;

        public int nRankIconID;

        public string RankName;
    }

    [Serializable]
    public class UInvitePlayGameTreeViewDataSource
    {
        /// <summary>
        /// The depth.
        /// </summary>
        [SerializeField]
        public int Depth;

        /// <summary>
        /// The is visible.
        /// </summary>
        [SerializeField]
        public bool IsVisible = true;

        /// <summary>
        /// The is expanded.
        /// </summary>
        [SerializeField]
        public bool IsExpanded;

        /// <summary>
        /// 头像.
        /// </summary>
        [SerializeField]
        public Sprite Icon;

        /// <summary>
        /// 分组名称和玩家名称共用该字段
        /// </summary>
        [SerializeField]
        public string Name;

        /// <summary>
        /// The tag.
        /// </summary>
        [SerializeField]
        public object Tag;

        public int nUserID;     

        public int nGroupID;

        public int nHeadIconID;

        public int nGameState;

        public int nSex;

        public int nRank;

        public string RankName;

        public bool bIsInvited;
    }


	public class InvitePlayGameWnd : UIPopupWnd<InvitePlayGameWndView>
	{
		public override WndID GetID()
		{
			return WndID.WND_ID_INVITEPLAYGAME;
		}

		public override WndLayerDef GetLayer()
		{
			return WndLayerDef.WND_LAYER_POPUP02;
		}

		public override string GetPrefabPath()
		{
			return "InvitePlayGame/InvitePlayGameWndView";
		}

		public override bool Init()
		{
			m_isKeyClose = true;

			UISystem.Instance.RegisterWndMessage(WndMsgID.WND_MSG_COMMOM_MATCHROOMSTATE_LEAVE, this);
			UISystem.Instance.RegisterWndMessage(WndMsgID.WND_MSG_COMMOM_STATICGAMESTATE_ENTER, this);
			UISystem.Instance.RegisterWndMessage(WndMsgID.WND_MSG_SNS_SHOW_INVITE_PLAYGAME_WND, this);
            UISystem.Instance.RegisterWndMessage(WndMsgID.WND_MSG_SNS_SHOW_INVITE_MATCH_WND, this);

			return base.Init();
		}

		public override void Destroy()
		{
			base.Destroy();
			UISystem.Instance.UnregisterWndMessage(WndMsgID.WND_MSG_COMMOM_MATCHROOMSTATE_LEAVE, this);
			UISystem.Instance.UnregisterWndMessage(WndMsgID.WND_MSG_COMMOM_STATICGAMESTATE_ENTER, this);
			UISystem.Instance.UnregisterWndMessage(WndMsgID.WND_MSG_SNS_SHOW_INVITE_PLAYGAME_WND, this);
            UISystem.Instance.UnregisterWndMessage(WndMsgID.WND_MSG_SNS_SHOW_INVITE_MATCH_WND, this);
		}

		public override void OnMessage(WndMsgID msgID, UIMsgData data)
		{
			switch (msgID)
			{
				case WndMsgID.WND_MSG_COMMOM_MATCHROOMSTATE_LEAVE:
					{
						// 进入登录界面,清理上一局数据
						if (m_wndView != null)
						{
							m_wndView.Clear();
						}

						UnloadView();
					}
					break;
				case WndMsgID.WND_MSG_COMMOM_STATICGAMESTATE_ENTER:
					{
						//SetVisible(true);
					}
					break;
				case WndMsgID.WND_MSG_SNS_SHOW_INVITE_PLAYGAME_WND: // 显示邀请一起游戏窗口
					{
						SetVisible(true);

                        if (m_wndView != null)
                        {
                            m_wndView.SetInviteGameMode();
                        }
					}
					break;
                case WndMsgID.WND_MSG_SNS_SHOW_INVITE_MATCH_WND: // 显示邀请一起游戏窗口
                    {
                        SetVisible(true);

                        if (m_wndView != null)
                        {
                            m_wndView.SetInviteTeamMode();
                        }
                    }
                    break;
				default:
					break;
			}
		}

		protected override void PostSetVisible(bool _bVisible)
		{
			base.PostSetVisible(_bVisible);

			if (_bVisible)
				m_wndView.updateContent();
		}
	}


	public class InvitePlayGameWndView : UIBaseWndView
    {
        public UInvitePlayGameTreeList InvitePlayGameTree;
		public Button SearchBtn;
		public Button InviteBtn;
        public Button InviteMatchTeamBtn;
		public Button InviteClanMemberBtn;
		public InputField SearchInputField;

		private List<UInvitePlayGameTreeViewDataSource> Data = new List<UInvitePlayGameTreeViewDataSource>();
		private string m_SearchText = "";

        // 已经邀请的记录
        private Dictionary<int, bool> m_InvitedRecord = new Dictionary<int, bool>();

        // 上次发送联盟邀请时间
        private float m_SendClanInviteTime = 0;
        public override bool Init(IUIWnd wnd)
        {
			base.Init(wnd);

            InvitePlayGameTree.Start();

			SearchBtn.onClick.AddListener(onSearchBtnClick);
			InviteBtn.onClick.AddListener(onInviteBtnClick);
            InviteMatchTeamBtn.onClick.AddListener(onInviteMatchTeamBtnClick);
            InvitePlayGameTree.OnSelect.AddListener(onSelectItemComponent);

            InviteClanMemberBtn.onClick.AddListener(onInviteClanMemberClick);
			return true;
        }
	
		public void Clear()
		{
			Data.Clear();
			m_SearchText = "";
            m_InvitedRecord.Clear();

            m_SendClanInviteTime = 0;
        }

        public void SetInviteGameMode()
        {
            InviteBtn.gameObject.SetActive(true);
            InviteMatchTeamBtn.gameObject.SetActive(false);
        }

        public void SetInviteTeamMode()
        {
            InviteBtn.gameObject.SetActive(false);
            InviteMatchTeamBtn.gameObject.SetActive(true);
        }

		public void onSearchBtnClick()
		{
			m_SearchText = SearchInputField.text;

			updateContent();
		}

		public void onInviteBtnClick()
		{
			var items = InvitePlayGameTree.DataSource;
			foreach(var item in items)
			{
				int nUserID = item.Node.Item.nUserID;
                bool isSelected = item.Node.Item.isSelected;
				if(isSelected && nUserID > 0)
				{
					int nRoomID = (int)GameLogicAPI.getCurrentRoomID();
					LogicDataCenter.snsDataManager.reqInvitePlayGame(nRoomID, nUserID);

                    // 邀请记录
                    if(!m_InvitedRecord.ContainsKey(nUserID))
                        m_InvitedRecord.Add(nUserID, true);
				}
			}

            // 刷新界面
            updateContent();
		}

        public void onInviteMatchTeamBtnClick()
        {
            var items = InvitePlayGameTree.DataSource;
            foreach (var item in items)
            {
                int nUserID = item.Node.Item.nUserID;
                bool isSelected = item.Node.Item.isSelected;
                if (isSelected && nUserID > 0)
                {
                    LogicDataCenter.snsDataManager.reqInviteMatchTeam(nUserID);

                    // 邀请记录
                    if (!m_InvitedRecord.ContainsKey(nUserID))
                        m_InvitedRecord.Add(nUserID, true);
                }
            }

            // 刷新界面
            updateContent();
        }

        public void onInviteClanMemberClick()
        {
            // 30秒cd
            float cdTime = Time.time - m_SendClanInviteTime;
            if (m_SendClanInviteTime > 0 && (cdTime < 30f))
            {
                UIUtil.ShowSystemMessage(EMChatTipID.CHAT_TIP_COMMON_OPT_CD_TIP, cdTime.ToString("F2"));
                return;
            }

            string str = String.Format(ULocalizationService.Instance.Get("UIView", "GameWaitingRoom", "ClanInviteMsg"),
                ChatMessageManager.ChatItemFlag, GameLogicAPI.getCurrentRoomID(), EntityUtil.getMainHeroName());

            UIUtil.SendChatMessage(EMChatChannelType.CHAT_CHANNEL_CLAN, str);

            m_SendClanInviteTime = Time.time;
        }

        private void onSelectItemComponent(int index, ListViewItem item)
		{
			UInvitePlayGameTreeItemComponent component = item as UInvitePlayGameTreeItemComponent;
			Debug.Log(index + ":" + component.Item.nGroupID + ":" + component.Item.nUserID);

			if(component.Item.nUserID <= 0)
			{
				InvitePlayGameTree.Deselect(index);
			}
		}

        /// <summary>
        /// Convert flat list to tree.
        /// </summary>
        /// <param name="nodes">Nodes.</param>
        public virtual void List2Tree(ObservableList<TreeNode<UInvitePlayGameTreeItemData>> nodes)
        {
            TreeNode<UInvitePlayGameTreeItemData> last_node = null;
            for (int i = 0; i < Data.Count; i++)
            {
                var item = Data[i];
                item.IsVisible = true;

                //Debug.Log(item.Depth + " -> " + item.Name + " -> " + item.IsVisible);
                if (item.Depth == 0)
                {
                    last_node = Item2Node(item);
                    nodes.Add(last_node);
                }
                else if (item.Depth == (Data[i - 1].Depth + 1))
                {
                    var current_node = Item2Node(item);
                    last_node.Nodes.Add(current_node);

                    last_node = current_node;
                }
                else if (item.Depth <= Data[i - 1].Depth)
                {
                    int n = item.Depth - Data[i - 1].Depth + 1;
                    for (int j = 0; j < n; j++)
                    {
                        last_node = last_node.Parent;
                    }

                    var current_node = Item2Node(item);
                    last_node.Nodes.Add(current_node);

                    last_node = current_node;
                }
                else
                {
                    //Debug.LogWarning("Unknown case");
                }
            }
        }

        /// <summary>
        /// Convert item to node.
        /// </summary>
        /// <returns>The node.</returns>
        /// <param name="item">Item.</param>
        protected virtual TreeNode<UInvitePlayGameTreeItemData> Item2Node(UInvitePlayGameTreeViewDataSource item)
        {
            var nodeItem = new UInvitePlayGameTreeItemData(item.Name, item.Icon);
            nodeItem.Tag = item.Tag;
            nodeItem.Name = item.Name;
            nodeItem.nUserID = item.nUserID;
            nodeItem.nGroupID = item.nGroupID;
            nodeItem.isSelected = false;
            nodeItem.nGameState = item.nGameState;
            nodeItem.nHeadIconID = item.nHeadIconID;
            nodeItem.nRank = item.nRank;
            nodeItem.RankName = item.RankName;
            nodeItem.nSex = item.nSex;
            nodeItem.isInvited = item.bIsInvited;

            return new TreeNode<UInvitePlayGameTreeItemData>(nodeItem, new ObservableList<TreeNode<UInvitePlayGameTreeItemData>>(), item.IsExpanded, item.IsVisible);
        }

        /// <summary>
        /// 设置好友分组数据源
        /// </summary>
        public void setInvitePlayGameTreeViewDataSource()
        {
            var nodes = new ObservableList<TreeNode<UInvitePlayGameTreeItemData>>();
            List2Tree(nodes);
            InvitePlayGameTree.Nodes = nodes;
        }


		/// <summary>
		/// 显示主面板
		/// </summary>
		public void updateContent()
        {
            // 设置好友列表内容
            Data.Clear();

			Dictionary<string, List<InvitePlayGamePlayerInfo>> groups = new Dictionary<string, List<InvitePlayGamePlayerInfo>>();

            // 在线好友
            List<InvitePlayGamePlayerInfo> group1 = new List<InvitePlayGamePlayerInfo>();
            foreach (var item in LogicDataCenter.snsDataManager.getOnlineBuddy())
            {
                InvitePlayGamePlayerInfo info = new InvitePlayGamePlayerInfo();
                info.nGroupID = (int)InvitePlayGameGroup.emBuddyGroup;
                info.nUserID = item.Info.nUserID;
                info.nGameState = item.Info.nStatus;
                info.ActorName = getBuddyName(item);
                info.nHeadIconID = item.Info.dwHeadID;
                info.nRankIconID = item.Info.nRankIconID;
                info.RankName = item.Info.szRankName;
                info.nSex = item.Info.nSex;

                group1.Add(info);
            }
            string groupName1 = ULocalizationService.Instance.Get("UIView", "Common", "OnlineBuddy");
            groups.Add(groupName1, group1);

            // 战队
            List<InvitePlayGamePlayerInfo> group2 = new List<InvitePlayGamePlayerInfo>();
            foreach (var item in LogicDataCenter.kinDataManager.KinMemberList)
            {
                if (item.dwPDBID == 0)
                    continue;

                // 排除自己
                if (item.dwPDBID == EntityUtil.getMainHeroPdbid())
                    continue;

                // 排除离线
                if (item.bIsOnline == 0)
                    continue;

                InvitePlayGamePlayerInfo info = new InvitePlayGamePlayerInfo();
                info.nUserID = item.dwUDBID;
                info.nGroupID = (int)InvitePlayGameGroup.emKinGroup;
                info.nGameState = item.byGameState;
                info.ActorName = item.szName;
                info.nHeadIconID = item.nFaceID;
                info.nRankIconID = 0;
                info.RankName = item.szGradeName;
                info.nSex = item.nSex;

                group2.Add(info);
            }
            string groupName2 = ULocalizationService.Instance.Get("UIView", "Common", "Team");
            groups.Add(groupName2, group2);

            // 附近的人
            List< InvitePlayGamePlayerInfo > group3 = new List<InvitePlayGamePlayerInfo>();
            foreach (var item in LogicDataCenter.snsDataManager.getNearbyPlayer())
            {
                InvitePlayGamePlayerInfo info = new InvitePlayGamePlayerInfo();
                info.nGroupID = (int)InvitePlayGameGroup.emNearbyGroup;
                info.nUserID = item.Info.nUserID;
                info.nGameState = item.Info.nStatus;
                info.ActorName = item.Info.szName;
                info.nHeadIconID = item.Info.dwHeadID;
                info.nRankIconID = item.Info.nRankIconID;
                info.RankName = item.Info.szRankName;
                info.nSex = item.Info.nSex;

                group3.Add(info);
            }
            string groupName3 = ULocalizationService.Instance.Get("UIView", "Common", "NearbyPeople");
            groups.Add(groupName3, group3);

            foreach (var group in groups)
            {
				// 添加一个分组
				UInvitePlayGameTreeViewDataSource data = new UInvitePlayGameTreeViewDataSource();
                data.Depth = 0;
				data.IsExpanded = true;
				data.Name = group.Key;
                
                Data.Add(data);

                // 添加该分组的玩家
                foreach (var player in group.Value)
                {
                    if (!isMatchSearchName(player.ActorName))
                        continue;

                    data = new UInvitePlayGameTreeViewDataSource();
                    data.Depth = 1;
                    data.nGroupID = player.nGroupID;
                    data.nUserID = player.nUserID;
                    data.nGameState = player.nGameState;
                    data.Name = player.ActorName;
                    data.nHeadIconID = player.nHeadIconID;
                    data.nRank = player.nRankIconID;
                    data.RankName = player.RankName;
                    data.nSex = player.nSex;
                    data.bIsInvited = isInvitedPlayer(player.nUserID);

                    Data.Add(data);
                }
            }

            setInvitePlayGameTreeViewDataSource();
        }
        
        // 是否已经邀请过该玩家
        private bool isInvitedPlayer(int nUserID)
        {
            return m_InvitedRecord.ContainsKey(nUserID);
        }

        private string getBuddyName(SNSDataManager.BuddyInfo buddyInfo)
        {
            string remark = LogicDataCenter.snsDataManager.getBuddyRemark(buddyInfo.Info.nUserID);

            remark = (remark == null ? "" : remark);

            if (remark.Length == 0)
            {
                return buddyInfo.Info.szName;
            }
            else
            {
                return string.Format("{0}（{1}）", remark, buddyInfo.Info.szName);
            }
        }

        // 玩家名字是否与搜索框的内容相同
        private bool isMatchSearchName(string name)
        {
            if (m_SearchText == "")
                return true;

            string pyName = PinYinConverter.Get(name).ToLower();
            string firstPYName = PinYinConverter.GetFirst(name).ToLower();
            name = name.ToLower();

            m_SearchText = m_SearchText.ToLower();

            if (name.IndexOf(m_SearchText) >= 0)
                return true;
            if (pyName.IndexOf(m_SearchText) >= 0)
                return true;
            if (firstPYName.IndexOf(m_SearchText) >= 0)
                return true;

            return false;
        }
    }
}