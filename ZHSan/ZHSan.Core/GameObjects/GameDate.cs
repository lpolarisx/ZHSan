using GameDatas;
using GameEnums;
using GameManager;
using System.Runtime.Serialization;

namespace GameObjects
{
    [DataContract]
    public class GameDate
    {
        [DataMember]
        public int Day { get; set; } = 1;

        [DataMember]
        public int DaysLeft { get; set; }

        [DataMember]
        public bool IsRunning { get; set; }

        [DataMember]
        public int Month { get; set; } = 1;

        [DataMember]
        public GameSeason Season { get; set; }

        [DataMember]
        public int Year { get; set; } = 184;

        public GameDate(GameDateConfig config)
        {
            Year = config.Year;
            Month = config.Month;
            Day = config.Day;
            Season = config.Season;
            DaysLeft = config.DaysLeft;
            IsRunning = config.IsRunning;
        }

        public GameDateConfig ToConfig()
        {
            return new GameDateConfig
            {
                Year = Year,
                Month = Month,
                Day = Day,
                Season = Season,
                DaysLeft = DaysLeft,
                IsRunning = IsRunning,
            };
        }

        public event DayPassedEvent OnDayPassed;

        public event DayRunningEvent OnDayRunning;

        public event DayStartingEvent OnDayStarting;

        public event MonthPassedEvent OnMonthPassed;

        public event MonthRunningEvent OnMonthRunning;

        public event MonthStartingEvent OnMonthStarting;

        public event SeasonChangeEvent OnSeasonChange;

        public event YearPassedEvent OnYearPassed;

        public event YearRunningEvent OnYearRunning;

        public event YearStartingEvent OnYearStarting;

        public bool EndRunning()
        {
            if (!this.IsRunning)
            {
                return false;
            }
            if ((this.OnDayPassed != null) && !this.OnDayPassed())
            {
                return false;
            }
            if (this.Day >= 30 - Session.Parameters.DayInTurn + 1)
            {
                if ((this.OnMonthPassed != null) && !this.OnMonthPassed())
                {
                    return false;
                }
                if ((this.Month >= 12) && ((this.OnYearPassed != null) && !this.OnYearPassed()))
                {
                    return false;
                }
            }

            IsRunning = false;
            return true;
        }

        public float GetFoodRateBySeason(GameSeason season)
        {
            switch (season)
            {
                case GameSeason.Spring:
                    return 0.6f;

                case GameSeason.Summer:
                    return 1f;

                case GameSeason.Autumn:
                    return 1f;

                case GameSeason.Winter:
                    return 0.3f;
            }
            return 0f;
        }

        public GameSeason GetSeason(int dayslater)
        {
            if (Day + dayslater > 30 && Month % 3 == 0)
            {
                return GameSeason.Spring + (int)Season % (int)GameSeason.Winter;
            }

            return Season;
        }

        public void Go()
        {
            //this.Day++;
            Day += Session.Parameters.DayInTurn;

            if (Day > 30)
            {
                //this.Day = 1;
                Day -= 30;
                Month++;

                if (Month > 12)
                {
                    Month = 1;
                    Year++;
                }

                SetSeason();
            }

            if (DaysLeft > 0)
            {
                DaysLeft--;
            }
        }

        public void Go(int i)
        {
            Day += i;

            while (Day > 30)
            {
                Day -= 30;
                Month++;

                if (Month > 12)
                {
                    Month = 1;
                    Year++;
                }

                SetSeason();
            }
        }

        public void LoadDateData(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
            SetSeason();
        }

        public void SetSeason()
        {
            GameSeason season = Season;

            if (Month >= 3 && Month <= 5)
            {
                Season = GameSeason.Spring;
            }
            else if (Month >= 6 && Month <= 8)
            {
                Season = GameSeason.Summer;
            }
            else if (Month >= 9 && Month <= 11)
            {
                Season = GameSeason.Autumn;
            }
            else
            {
                this.Season = GameSeason.Winter;
            }
            if ((season != this.Season) && (this.OnSeasonChange != null))
            {
                this.OnSeasonChange(this.Season);
            }
            //*jokosany每个月重新随机选择一首背景音乐,必须放在this.SetSeason();之后
            //Session.MainGame.mainGameScreen.SwichMusic(Session.Current.Scenario.Date.Season);
            //  Session.MainGame.mainGameScreen.SwichMusic(GameSeason.秋);
            // Session.MainGame.mainGameScreen.SwichMusic(this.Season);
        }

        public bool StartRunning()
        {
            if (IsRunning) return false;

            if (OnDayStarting != null && !OnDayStarting()) return false;

            if (Day <= Session.Current.Scenario.Parameters.DayInTurn)
            {
                if (OnMonthStarting != null && !OnMonthStarting()) return false;

                if (Month == 1 && OnYearStarting != null && !OnYearStarting()) return false;
            }

            IsRunning = true;
            return true;
        }

        public string ToDateString()
        {
            return ToString();
        }

        public override string ToString() => $"{Year}年{Month}月{Day}日";

        public int LeftDays => 360 - PassedDays;

        public int PassedDays => Month * 30 + Day;

        public GameDate() { }

        public GameDate(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public GameDate(GameDate d)
        {
            Year = d.Year;
            Month = d.Month;
            Day = d.Day;
        }
        
        public delegate bool DayPassedEvent();

        public delegate bool DayRunningEvent();

        public delegate bool DayStartingEvent();

        public delegate bool MonthPassedEvent();

        public delegate bool MonthRunningEvent();

        public delegate bool MonthStartingEvent();

        public delegate void SeasonChangeEvent(GameSeason season);

        public delegate bool YearPassedEvent();

        public delegate bool YearRunningEvent();

        public delegate bool YearStartingEvent();
    }
}