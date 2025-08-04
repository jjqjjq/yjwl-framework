using System.Collections.Generic;
using JQCore.tSingleton;

namespace JQCore.tTime
{
    public class TickMgr : JQSingleton<TickMgr>
    {
        private List<ITick> _tickList = new List<ITick>();
        private List<ILateTick> _lateTickList = new List<ILateTick>();
        private List<IFixedTick> _fixedTickList = new List<IFixedTick>();
        private List<ITick> _tempList = new List<ITick>();
        private List<ILateTick> _lateTempList = new List<ILateTick>();
        private List<IFixedTick> _fixedTempList = new List<IFixedTick>();


        private bool _listChange = false;
        private bool _lateListChange = false;
        private bool _fixedListChange = false;

        private void updateTempList<T>(List<T> tempList, List<T> list)
        {
            tempList.Clear();
            tempList.AddRange(list);
        }

        public override void Dispose()
        {
            _tickList.Clear();
            _lateTickList.Clear();
            _fixedTickList.Clear();
            _tempList.Clear();
            _lateTempList.Clear();
            _fixedTempList.Clear();
        }


        public void onTick()
        {
            if (_listChange)
            {
                updateTempList(_tempList, _tickList);
                _listChange = false;
            }

            for (int i = 0; i < _tempList.Count; i++)
            {
                _tempList[i].onTick();
            }
        }

        public void onFixedTick()
        {
            if (_fixedListChange)
            {
                updateTempList(_fixedTempList, _fixedTickList);
                _fixedListChange = false;
            }

            for (int i = 0; i < _fixedTempList.Count; i++)
            {
                _fixedTempList[i].onFixedTick();
            }
        }

        public void onLateTick()
        {
            if (_lateListChange)
            {
                updateTempList(_lateTempList, _lateTickList);
                _lateListChange = false;
            }

            for (int i = 0; i < _lateTempList.Count; i++)
            {
                _lateTempList[i].onLateTick();
            }
        }

        public void addTick(object tick)
        {
            if (tick is ITick)
            {
                ITick itick = tick as ITick;
                _listChange = true;
                if (!_tickList.Contains(itick))
                {
                    _tickList.Add(itick);
                }
            }

            if (tick is ILateTick)
            {
                ILateTick itick = tick as ILateTick;
                _lateListChange = true;
                if (!_lateTickList.Contains(itick))
                {
                    _lateTickList.Add(itick);
                }
            }

            if (tick is IFixedTick)
            {
                IFixedTick itick = tick as IFixedTick;
                _fixedListChange = true;
                if (!_fixedTickList.Contains(itick))
                {
                    _fixedTickList.Add(itick);
                }
            }
        }

        public void removeTick(object tick)
        {
            if (tick is ITick)
            {
                ITick itick = tick as ITick;
                _listChange = true;
                _tickList.Remove(itick);
            }

            if (tick is ILateTick)
            {
                ILateTick itick = tick as ILateTick;
                _lateListChange = true;
                _lateTickList.Remove(itick);
            }

            if (tick is IFixedTick)
            {
                IFixedTick itick = tick as IFixedTick;
                _fixedListChange = true;
                _fixedTickList.Remove(itick);
            }
        }
    }
}