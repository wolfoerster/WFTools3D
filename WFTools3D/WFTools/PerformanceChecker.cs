//******************************************************************************************
// Copyright © 2016 - 2022 Wolfgang Foerster (wolfoerster@gmx.de)
//
// This file is part of the WFTools3D project which can be found on github.com.
//
// WFTools3D is free software: you can redistribute it and/or modify it under the terms 
// of the GNU General Public License as published by the Free Software Foundation, 
// either version 3 of the License, or (at your option) any later version.
// 
// WFTools3D is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//******************************************************************************************

namespace WFTools3D
{
    using System.Diagnostics;

    public class PerformanceChecker
    {
        private readonly Stopwatch watch = new Stopwatch();
        private long count;
        private long prevCount;

        public string GetResult()
        {
            if (!watch.IsRunning)
            {
                Reset();
                return "";
            }

            elapsed = watch.ElapsedMilliseconds;
            if (elapsed < 3)
                return "";

            watch.Reset();
            watch.Start();
            average = (average * count + elapsed) / ++count;
            return string.Format("Average: {0:F0} ms, {1:F0} fps", average, 1e3 / average);
        }

        public string GetResult(long simulatorCount)
        {
            string msg = GetResult();
            long diff = simulatorCount - prevCount;
            prevCount = simulatorCount;
            return string.Format("{0}, {1} cpms", msg, diff / elapsed);
        }

        public void Reset()
        {
            count = 0;
            average = 0;
            watch.Reset();
            watch.Start();
        }

        public long Elapsed
        {
            get { return elapsed; }
        }
        long elapsed;

        public double Average
        {
            get { return average; }
        }
        double average;
    }
}
