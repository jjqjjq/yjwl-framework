

namespace JQEditor.Check
{
    public static class CheckDataTools
    {
//        private static void checkDropData(uint id, uint goodsId)
//        {
//            DropData data = DropData.getDataByID(id, false);
//            if (data == null)
//            {
//                Debug.LogError("不存在的dropData： Id = " + id + " === pos : 物品表 ID=  " + goodsId);
//            }
//        }
//
//        private static void checkGoodsDataUseAction(string jsonStr, GoodsData goodsData)
//        {
//            JSONObject jsonObject = new JSONObject(jsonStr);
//            for (int i = 0; i < jsonObject.keys.Count; i++)
//            {
//                string key = jsonObject.keys[i];
//
//                switch (key)
//                {
//                    case EJson.USE_TYPE_DROPDATA:
//                        uint id = uint.Parse(jsonObject.GetField(key).str);
//                        checkDropData(id, goodsData.id);
//                        break;
//                }
//            }
//        }
//
//        
//        private static void checkGoodsDataPasscityAction(string jsonStr, GoodsData goodsData)
//        {
//            JSONObject jsonObject = new JSONObject(jsonStr);
//            for (int i = 0; i < jsonObject.keys.Count; i++)
//            {
//                string key = jsonObject.keys[i];
//
//                switch (key)
//                {
//                    case EJson.P_TYPE_MAGIC_BAG:
//                        uint[] paramsArr = ConfigTool.getArrUInt(jsonObject.GetField(key).str);;
//                        checkDropData(paramsArr[1], goodsData.id);
//                        break;
//                }
//            }
//        }
//
//
//        //[MenuItem("IrfCheck/Data/GoodsData")]
//        public static void checkDropData()
//        {
//            GameDataUtil.initData();
//
//            //物品表 
//            List<GoodsData> goodsList = new List<GoodsData>(GoodsData.dataMap.Values);
//            for (int i = 0; i < goodsList.Count; i++)
//            {
//                GoodsData goodsData = goodsList[i];
//                checkGoodsDataUseAction(goodsData.use_actions, goodsData);
//                checkGoodsDataUseAction(goodsData.pickUp_actions, goodsData);
//                checkGoodsDataPasscityAction(goodsData.passivity_actions, goodsData);
//            }
//
//            //AI 节点
//            ScriptableObject[] aiDic = AIManager.getInstance().AiArr;
//            for (int i = 0; i < aiDic.Length; i++)
//            {
//                BehaviourTree tree = aiDic[i] as BehaviourTree;
//                List<Node> allNodes = tree.allNodes;
//                for (int j = 0; j < allNodes.Count; j++)
//                {
//                    if (!(allNodes[j] is ITaskAssignable)) continue;
//                    if ((allNodes[j] as ITaskAssignable).task is CreateDropRuleAction)
//                    {
//                        CreateDropRuleAction createDropRuleAction = (allNodes[j] as ITaskAssignable).task as CreateDropRuleAction;
//                        uint dropRuleId = (uint)createDropRuleAction.dropRuleId;
//                        DropData data = DropData.getDataByID(dropRuleId, false);
//                        if (data == null)
//                        {
//                            Debug.LogError("不存在的dropData： Id = " + dropRuleId + " === pos : ai name =  " + aiDic[i].name);
//                        }
//
//                    }
//                }
//            }

//        }
    }
}
