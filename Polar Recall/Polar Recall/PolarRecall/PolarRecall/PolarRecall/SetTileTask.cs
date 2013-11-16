using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.TaskManagement;
using Engine.Map;

namespace PolarRecall
{
    class SetTileTask : Task
    {
        #region Fields
        char _tile;
        int _x;
        int _y;
        Level _level;
        #endregion

        public SetTileTask(Level level, int x, int y, char tile)
            : base(TaskType.GameSpecific)
        {
            _level = level;
            _x = x;
            _y = y;
            _tile = tile;
        }

        public override void Update(int milliseconds)
        {
            base.Update(milliseconds);

            _level.SetTileAt(_x, _y, _tile);
            
            Kill();
        }
    }
}
