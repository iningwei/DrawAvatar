using System;


public class TimeTool
{
    /// <summary>
    /// 获取当前周的星期一和星期日
    /// </summary>
    public static (DateTime Monday, DateTime Sunday) GetCurrentWeek()
    {
        DateTime today = DateTime.Now.Date;  // 只取日期部分

        // 计算今天是星期几（0=周日，1=周一，...，6=周六）
        int dayOfWeek = (int)today.DayOfWeek;

        // 计算星期一：如果今天是周日，dayOfWeek=0，需要往前推6天
        DateTime monday = today.AddDays(dayOfWeek == 0 ? -6 : -dayOfWeek + 1);

        DateTime sunday = monday.AddDays(6);

        return (monday, sunday);
    }



    //判断当前时间是否在某两个日期的时间断内
    //startTimeStr,形如：2024-03-01
    //endtTimeStr,形如：2024-04-21
    public static bool IsSuitDayDuration(string startDayStr, string endDatStr)
    {
        DateTime startTime = DateTime.Parse(startDayStr);
        DateTime endTime = DateTime.Parse(endDatStr);

        //当前时间
        DateTime curTime = DateTime.Now;
        if (curTime >= startTime && curTime <= endTime)
        {
            return true;
        }
        return false;
    }
    public static bool IsSuitDayDuration(string startDayStr, int addedDayCount)
    {
        DateTime startTime = DateTime.ParseExact(startDayStr, "yyyy-MM-dd", null);
        DateTime newDateTime = startTime.AddDays(addedDayCount);
        string endDay = newDateTime.ToString("yyyy-MM-dd");
        return IsSuitDayDuration(startDayStr, endDay);
    }

    //startTimeStr，形如：14:00:00
    //endTimeStr,形如：24:00:00
    public static bool IsSuitTimeDuration(string startTimeStr, string endTimeStr)
    {
        // 将时间字符串解析为 TimeSpan 对象
        TimeSpan startTime = TimeSpan.Parse(startTimeStr);
        TimeSpan endTime = TimeSpan.Parse(endTimeStr);

        // 获取当前时间的时间部分
        TimeSpan currentTime = DateTime.Now.TimeOfDay;

        if (currentTime >= startTime && currentTime <= endTime)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// 获得当前时间对应的时间戳（单位毫秒）
    /// </summary>
    /// <returns></returns>
    public static long GetNowStamp()
    {
        return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
    }
    /// <summary>
    /// 获得当前时间对应的时间戳（单位秒）
    /// </summary>
    /// <returns></returns>
    public static long GetNowSecondStamp()
    {
        return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
    }
    /// <summary>
    /// 获得明天0点的时间戳(单位毫秒)
    /// </summary>
    /// <returns></returns>
    public static long GetTomorrowZeroTimeStamp()
    {
        //获得今天0点对应的DateTime
        var todayZeroTime = DateTime.Now.Date;
        //获得明天0点对应的DateTime
        var tomorrowZeroTime = todayZeroTime.AddDays(1);

        long tomorrowStamp = (tomorrowZeroTime.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        return tomorrowStamp;
    }



    /// <summary>
    /// 获得年月日零点对应的时间戳(精确度根据参数)
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    /// <returns></returns>
    public static long GetStampByyyyyMMdd(int year, int month, int day, bool accurateToMilliseconds)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
        DateTime targetTime = new System.DateTime(year, month, day);

        if (accurateToMilliseconds)
        {
            return (long)(targetTime - startTime).TotalMilliseconds;
        }
        else
        {
            return (long)(targetTime - startTime).TotalSeconds;
        }

    }


    /// <summary>
    /// 根据时间戳获得对应的年月日
    /// </summary>
    /// <param name="inStamp"></param>
    /// <param name="isStampAccurateToMilliseconds">inStamp的精度是否是毫秒</param>
    /// <param name="split"></param>
    /// <returns></returns>
    public static string GetyyyyMMddByStamp(long inStamp, bool isStampAccurateToMilliseconds, string split)
    {
        //先转换为DateTime
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
        DateTime time;
        if (isStampAccurateToMilliseconds)
        {
            time = startTime.AddTicks(inStamp * 10000);
        }
        else
        {
            time = startTime.AddTicks(inStamp * 10000000);
        }


        //转换为年月日
        return GetyyyyMMdd(time, split);
    }

    /// <summary>
    /// 根据时间戳获得对应的年月日时分
    /// </summary>
    /// <param name="inStamp"></param>
    /// <param name="isStampAccurateToMilliseconds">inStamp的精度是否是毫秒</param>
    /// <param name="split"></param>
    /// <returns></returns>
    public static string GetyyyyMMddHHmmByStamp(long inStamp, bool isStampAccurateToMilliseconds, string ymdSplit, string hmSplit)
    {
        //先转换为DateTime
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
        DateTime time;
        if (isStampAccurateToMilliseconds)
        {
            time = startTime.AddTicks(inStamp * 10000);
        }
        else
        {
            time = startTime.AddTicks(inStamp * 10000000);
        }

        var value = GetYMDHMSMs(time);

        return $"{value[0]}{ymdSplit}{value[1].ToString("D2")}{ymdSplit}{value[2].ToString("D2")} {value[3].ToString("D2")}{hmSplit}{value[4].ToString("D2")}";
    }

    /// <summary>
    /// 获得下周一 0点对应的时间戳(单位毫秒)
    /// </summary>
    /// <returns></returns>
    public static long GetNextMondayZeroTimeStamp()
    {
        int dayInt = 1;
        var dw = DateTime.Now.DayOfWeek;
        if (dw == 0)
        {
            dayInt = 7;
        }
        else
        {
            dayInt = (int)dw;
        }

        //从今天0点到下周一0点需要增加的天数
        int dayOffset = 7 - dayInt + 1;


        //获得今天0点对应的DateTime
        var todayZeroTime = DateTime.Now.Date;
        var targetZeroTime = todayZeroTime.AddDays(dayOffset);
        long targetStamp = (targetZeroTime.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        return targetStamp;
    }

    public static long[] FormatSecondsToDHMS(long seconds)
    {
        long day = 0;
        long h = 0;
        long m = 0;
        long s = 0;

        day = seconds / (3600 * 24);
        seconds = seconds - (day * 3600 * 24);
        h = seconds / 3600;
        seconds = seconds - h * 3600;
        m = seconds / 60;
        s = seconds - m * 60;
        return new long[4] { day, h, m, s };
    }

    /// <summary>
    /// 格式化秒数量
    /// 1d 24h 36m 43s
    /// </summary>
    /// <returns></returns>
    public static string FormatSeconds(int seconds)
    {
        int day = 0;
        int h = 0;
        int m = 0;
        int s = 0;

        day = seconds / (3600 * 24);
        seconds = seconds - (day * 3600 * 24);
        h = seconds / 3600;
        seconds = seconds - h * 3600;
        m = seconds / 60;
        s = seconds - m * 60;
        if (day > 0)
        {
            return $"{day}d {h}h {m}m {s}s";
        }
        else if (h > 0)
        {
            return $"{h}h {m}m {s}s";
        }
        else
        {
            return $"{m}m {s}s";
        }
    }


    /// <summary>
    /// 格式化秒
    /// 3:2:1:1 (天:小时:分钟:秒)
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static string FormatSeconds2(long seconds)
    {
        long day = seconds / (3600 * 24);
        long h = seconds % (3600 * 24) / 3600;
        long m = seconds % 3600 / 60;
        long s = seconds % 60;
        if (day > 0)
        {
            return $"{day}:{h}:{m}:{s}";
        }
        else if (h > 0)
        {
            return $"{h}:{m}:{s}";
        }
        else
        {
            //输出为：00:00
            return $"{m,2:00}:{s,2:00}";
        }
    }

    /// <summary>
    /// 格式化秒
    /// 3:2:1 (小时:分钟:秒)
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static string FormatSeconds3(long seconds)
    {
        long h = seconds / 3600;
        long m = seconds % 3600 / 60;
        long s = seconds % 60;
        //输出为：00:00
        return $"{h,2:00}:{m,2:00}:{s,2:00}";
    }

    /// <summary>
    /// 格式化秒
    /// 03小时02分01秒 (小时:分钟:秒) or 52分01秒 (分钟:秒)
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="usedHour"></param>
    /// <returns></returns>
    public static string FormatSeconds4(long seconds,bool isSimple,bool usedHour)
    {
        if (usedHour)
        {
            long h = seconds / 3600;
            long m = seconds % 3600 / 60;
            long s = seconds % 60;
            //00:00:00
            if (isSimple)
            {
                return $"{h,2:00}:{m,2:00}:{s,2:00}";
            }

            return $"{h,2:00}小时{m,2:00}分{s,2:00}秒";
        }
        else
        {
            long m = seconds  / 60;
            long s = seconds % 60;
            //00:00
            if (isSimple)
            {
                return $"{m,2:00}:{s,2:00}";
            }
            return $"{m,2:00}分{s,2:00}秒";
        }
    }

    /// <summary>
    /// 单位毫秒
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static long GetStamp(DateTime date)
    {
        return (date.ToUniversalTime().Ticks - 621355968000000000) / 10000;
    }

    /// <summary>
    /// 获得年月日字符串
    /// 格式：yyyy分隔符MM分隔符dd
    /// </summary>
    /// <param name="date"></param>
    /// <param name="splitStr"></param>
    /// <returns></returns>
    public static string GetyyyyMMdd(DateTime date, string splitStr)
    {
        return date.ToString($"yyyy{splitStr}MM{splitStr}dd");
    }

    public static string GetyyyyMMddHHmm(DateTime date, string splitStr)
    {
        return date.ToString($"yyyy{splitStr}MM{splitStr}dd{splitStr}HH{splitStr}mm");
    }

    /// <summary>
    /// 时间格式转换为年月日时分秒毫秒 (24小时)
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static string GetyyyyMMddHHmmssfff(DateTime dt, bool split = false)
    {
        if (split == false)
        {
            return dt.ToString("yyyyMMddHHmmssfff");//24小时
        }

        string yyyyMMddSplit = "/";
        string gapSplit = " ";
        string timeSplit = ":";
        return dt.ToString($"yyyy{yyyyMMddSplit}MM{yyyyMMddSplit}dd{gapSplit}HH{timeSplit}mm{timeSplit}ss{timeSplit}fff");
    }


    public static string GetyyyyMMddHHmmssfffWithSplit(DateTime dt, string split = "-")
    {
        return dt.ToString($"yyyy{split}MM{split}dd{split}HH{split}mm{split}ss{split}fff");
    }


    /// <summary>
    /// 时间格式转换为年月日时分秒毫秒 (12小时)
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static string GetyyyyMMddhhmmssfff(DateTime dt)
    {
        return dt.ToString("yyyyMMddhhmmssfff");//12小时
    }

    /// <summary>
    /// 获得：年 月 日 小时 分钟 秒 毫秒 数值存在int[]数组内
    /// [0]是年，[1]是月，[2]是日
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int[] GetYMDHMSMs(DateTime date)
    {
        int[] resultArray = new int[7];
        resultArray[0] = date.Year;
        resultArray[1] = date.Month;
        resultArray[2] = date.Day;
        resultArray[3] = date.Hour;
        resultArray[4] = date.Minute;
        resultArray[5] = date.Second;
        resultArray[6] = date.Millisecond;
        return resultArray;
    }

    public static int[] GetYMDHMS(long seconds)
    {
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(seconds).LocalDateTime;
        int[] resultArray = new int[6];
        resultArray[0] = dateTime.Year;
        resultArray[1] = dateTime.Month;
        resultArray[2] = dateTime.Day;
        resultArray[3] = dateTime.Hour;
        resultArray[4] = dateTime.Minute;
        resultArray[5] = dateTime.Second;
        return resultArray;
    }

    /// <summary>
    /// 比较 旧的年月日 和 新的年月日
    /// 满足新旧关系返回1
    /// 新旧相等返回0
    /// 不满足新旧关系返回-1
    /// </summary>
    /// <param name="oldYMD"></param>
    /// <param name="newYMD"></param>
    /// <returns></returns>
    public static int CompareTwoYMD(int[] oldYMD, int[] newYMD)
    {
        int oldYear = oldYMD[0];
        int oldMonth = oldYMD[1];
        int oldDay = oldYMD[2];
        int newYear = newYMD[0];
        int newMonth = newYMD[1];
        int newDay = newYMD[2];

        if (oldYear == newYear && oldMonth == newMonth && oldDay == newDay)
        {
            return 0;
        }

        if (oldYear > newYear)
        {
            return -1;
        }
        else
        {
            if (oldMonth > newMonth)
            {
                return -1;
            }
            else
            {
                if (oldDay > newDay)
                {
                    return -1;
                }
            }
        }

        return 1;
    }


    /// <summary> 
    /// 判断两个日期是否在同一周 
    /// </summary> 
    /// <param name="startTime"> 开始日期 </param> 
    /// <param name="endTime"> 结束日期 </param>
    /// <returns></returns> 
    public static bool IsInSameWeek(DateTime startTime, DateTime endTime)
    {
        double dbl = 0;
        TimeSpan ts = endTime - startTime;
        if (ts.TotalMilliseconds < 0)
        {
            DateTime tmp = startTime;
            startTime = endTime;
            endTime = tmp;
            dbl = -ts.TotalDays;
        }
        else
        {
            dbl = ts.TotalDays;
        }
        ///周日的话，对应的枚举是0，故要转成7
        int intDow = Convert.ToInt32(endTime.DayOfWeek);
        if (intDow == 0) intDow = 7;
        if (dbl >= 7 || dbl >= intDow) return false;
        else return true;
    }




    //https://blog.csdn.net/Rhett_Yuan/article/details/106022289
    /// <summary>
    /// 获得时区
    /// </summary>
    /// <returns></returns>
    public static int GetTimeZone()
    {
        return System.TimeZone.CurrentTimeZone.GetUtcOffset(System.DateTime.Now).Hours;
    }

    //ref:http://t.zoukankan.com/jietian331-p-7717548.html
    public enum Constellation
    {
        Aquarius = 1,       // 水瓶座 1.20 - 2.18
        Pisces = 2,         // 双鱼座 2.19 - 3.20
        Aries = 3,          // 白羊座 3.21 - 4.20
        Taurus = 4,         // 金牛座 4.21 - 5.20
        Gemini = 5,         // 双子座 5.21 - 6.21
        Cancer = 6,         // 巨蟹座 6.22 - 7.22
        Leo = 7,            // 狮子座 7.23 - 8.22
        Virgo = 8,          // 处女座 8.23 - 9.22
        Libra = 9,          // 天秤座 9.23 - 10.22
        Acrab = 10,         // 天蝎座 10.23 - 11.21
        Sagittarius = 11,   // 射手座 11.22 - 12.21
        Capricornus = 12,   // 摩羯座 12.22 - 1.19
    }


    /// <summary>
    /// 根据出生日期获得星座信息
    /// </summary>
    /// <param name="birthMonth">月</param>
    /// <param name="birthDate">日</param>
    /// <returns></returns>
    public static int GetConstellation(int birthMonth, int birthDate)
    {
        float birthdayF = birthMonth == 1 && birthDate < 20 ?
            13 + birthDate / 100f :
            birthMonth + birthDate / 100f;

        float[] bound = { 1.20F, 2.19F, 3.21F, 4.21F, 5.21F, 6.22F, 7.23F, 8.23F, 9.23F, 10.23F, 11.22F, 12.22F, 13.20F };

        Constellation[] constellations = new Constellation[12];
        for (int i = 0; i < constellations.Length; i++)
            constellations[i] = (Constellation)(i + 1);

        for (int i = 0; i < bound.Length - 1; i++)
        {
            float b = bound[i];
            float nextB = bound[i + 1];
            if (birthdayF >= b && birthdayF < nextB)
                return (int)constellations[i];
        }

        return (int)Constellation.Acrab;
    }


    /// <summary>
    /// 计算生日还有多少天 ref:https://blog.csdn.net/qq_34576513/article/details/107819158
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    /// <param name="calcLeapYear">是否计算闰年</param>
    /// <returns></returns>
    public static int CalculationBirthday(int year, int month, int day, bool calcLeapYear) // 天数
    {
        if (calcLeapYear)
        {
            return dCalc(year, month, day);
        }
        else
        {
            DateTime today = DateTime.Now;
            DateTime nextBirthday = new DateTime(today.Year, month, day);
            if (nextBirthday < today)
            {
                //今年生日已经过了计算明年的
                nextBirthday = new DateTime(today.Year + 1, month, day);
            }
            return nextBirthday.Subtract(today).Days;
        }

    }

    /// <summary>
    /// 计算生日
    /// </summary>
    /// <param name="y"></param>
    /// <param name="m"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    private static int getDay(int y, int m, int d)
    {
        int r = 0;
        int[] dm = { 333, 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 303 };
        y += (m - 1) / 12;
        m %= 12;
        // 计算闰年
        if ((y % 400 == 0) || ((y % 4 == 0) && (y % 100 != 0)))
        {
            r = 1;
        }
        if (r == 1 && (1 != m) && (2 != m))
            d++; // 处理闰年二月后日期
        y--;
        return (y * 365 + dm[m] + d + y / 4 - y / 100 + y / 400);
    }
    private static int yCalc(int y, int m, int d) // 年龄
    {
        int y0 = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
        int m0 = Convert.ToInt32(DateTime.Now.ToString("MM"));
        int d0 = Convert.ToInt32(DateTime.Now.ToString("dd"));
        return (getDay(y0, m0, d0) - getDay(y, m, d)) / 365;
    }
    private static int dCalc(int y, int m, int d) // 天数
    {
        int y0 = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
        int m0 = Convert.ToInt32(DateTime.Now.ToString("MM"));
        int d0 = Convert.ToInt32(DateTime.Now.ToString("dd"));
        int d1, d2;
        d1 = getDay(y, m, d); // 今年生日
        d2 = getDay(y, m0, d0); // 今天zhi
        if (d1 < d2) d1 = getDay(y + 1, m, d); // 今年生日过去了, 算明年生日
        return d1 - d2;
    }

}