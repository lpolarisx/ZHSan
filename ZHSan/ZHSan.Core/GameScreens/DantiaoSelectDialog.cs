using GameGlobal;
using GameManager;
using GameObjects;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;
using static GameManager.MyraWidgets;

namespace WorldOfTheThreeKingdoms.GameScreens
{
    /// <summary>
    /// 主菜单的单挑：从一个剧本里随机挑28名武将排成4行7列。按“随机”后选框在格子上跑，
    /// 给左右两方各抽1到3名武将；抽好后出现“开始”，按下进入单挑，每方武力最高的为主将，其余为候补。
    /// 剧本取全局设置DantiaoScenario，为空或找不到时随机选一个。每次打开都重新挑28人。
    /// 用Myra绘制，底板和按钮取自MyraSkin的基础图。
    /// </summary>
    internal class DantiaoSelectDialog
    {
        private const int Rows = 4;
        private const int Columns = 7;
        private const int MaxPerSide = 3;

        private const int DialogPadding = 28;
        private const int BodySpacing = 16;
        private const int CellWidth = 76;
        private const int CellHeight = 108;
        private const int CellSpacing = 6;
        private const int PortraitWidth = 64;
        private const int PortraitHeight = 80;
        private const int GridWidth = Columns * CellWidth + (Columns - 1) * CellSpacing;
        private const int GridHeight = Rows * CellHeight + (Rows - 1) * CellSpacing;
        private const int SideWidth = 210;
        private const int SlotHeight = 96;
        // 剧本名一行加间距，两边的阵营栏与中间一样高
        private const int CenterHeaderHeight = 30;
        private const int BodyHeight = CenterHeaderHeight + GridHeight;

        // 选框每抽一人在格子上跳几下，越跳越慢，停下的格子就是抽中的
        private const int MinHops = 6;
        private const int MaxHops = 9;
        private const float FirstHopInterval = 0.04f;
        private const float HopSlowdown = 1.16f;
        private const float PauseAfterPick = 0.3f;

        // 左方红、右方蓝，与单挑画面一致
        private static readonly Color[] SideColors = { new Color(0xC7, 0x5A, 0x4C), new Color(0x52, 0x88, 0xC6) };
        private static readonly string[] SideNames = { "红　方", "蓝　方" };
        private const string BackgroundTexture = @"Content\Textures\Resources\Start\bg.png";

        private static readonly IBrush ShadeBrush = new SolidBrush(new Color(0, 0, 0, 150));

        /// <summary>
        /// 正在显示的弹窗，主菜单每帧调它的Update推进读取和抽签
        /// </summary>
        public static DantiaoSelectDialog Current { get; private set; }

        private readonly Action<Scenario, List<Person>, List<Person>> onStart;
        private readonly Random rng = new Random();
        private readonly List<Cell> cells = new List<Cell>();
        private readonly List<Person>[] teams = { new List<Person>(), new List<Person>() };
        private readonly VerticalStackPanel[] slotPanels = new VerticalStackPanel[2];
        private readonly Queue<(int Side, int Cell)> pending = new Queue<(int Side, int Cell)>();

        private Widget root;
        private VerticalStackPanel gridHost;
        private Label scenarioLabel;
        private Label hintLabel;
        private Button randomButton;
        private Label randomLabel;
        private Button startButton;

        private Scenario scenario;
        private bool loadFailed;
        private bool built;

        private bool rolling;
        private int cursor = -1;
        private int hopsLeft;
        private float hopInterval;
        private float hopTimer;

        private class Cell
        {
            public Person Person;
            public Panel Frame;
            public Panel Shade;
            public int Side = -1;
        }

        private DantiaoSelectDialog(Action<Scenario, List<Person>, List<Person>> onStart)
        {
            this.onStart = onStart;
        }

        /// <summary>
        /// 显示弹窗
        /// </summary>
        /// <param name="onStart">点开始后调用：剧本、左方武将、右方武将，主将在前</param>
        public static void Show(Action<Scenario, List<Person>, List<Person>> onStart)
        {
            var dialog = new DantiaoSelectDialog(onStart);
            dialog.root = dialog.Build();
            Current = dialog;
            MyraUI.Show(dialog.root, () => dialog.Close(null));
            dialog.StartLoading();
        }

        private void Close(Action afterClose)
        {
            if (Current == this)
            {
                Current = null;
            }
            MyraUI.Close(afterClose);
        }

        #region 界面

        private Widget Build()
        {
            var dialog = new VerticalStackPanel
            {
                Spacing = 10,
                // 上边留得少，让标题落在底板那条横线的上面
                Padding = new Thickness(DialogPadding, 7, DialogPadding, DialogPadding),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = MyraSkin.PanelBackground
            };

            var titleRow = new Panel
            {
                Height = MyraSkin.PanelTitleHeight - 7
            };
            titleRow.Widgets.Add(new Label
            {
                Text = "单　挑",
                Font = TextManager.GetFont(24),
                TextColor = MyraSkin.TextTitle,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
            titleRow.Widgets.Add(CreateCloseButton(() => this.Close(null)));
            dialog.Widgets.Add(titleRow);

            var body = new HorizontalStackPanel
            {
                Spacing = BodySpacing
            };
            body.Widgets.Add(this.BuildSide(0));
            body.Widgets.Add(this.BuildCenter());
            body.Widgets.Add(this.BuildSide(1));
            dialog.Widgets.Add(body);

            this.hintLabel = new Label
            {
                Font = TextManager.GetFont(15),
                TextColor = MyraSkin.TextSecondary,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            dialog.Widgets.Add(this.hintLabel);

            var buttons = new HorizontalStackPanel
            {
                Spacing = 40,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            this.randomButton = MyraSkin.CreateButton("随机", 110, out this.randomLabel);
            this.randomButton.Click += (sender, args) => this.Roll();
            this.startButton = MyraSkin.CreateButton("开始", 110, out _);
            this.startButton.Click += (sender, args) => this.Start();
            this.startButton.Visible = false;
            buttons.Widgets.Add(this.randomButton);
            buttons.Widgets.Add(this.startButton);
            dialog.Widgets.Add(buttons);

            this.SetRandomEnabled(false);

            // 背景图铺满窗口，盖住后面的主菜单；找不到图时退回半透明遮罩
            var background = CacheManager.LoadTexture(BackgroundTexture);
            var mask = new Panel
            {
                Background = background != null ? new TextureRegion(background) : new SolidBrush(new Color(0, 0, 0, 150))
            };
            mask.Widgets.Add(dialog);
            return mask;
        }

        private Widget BuildSide(int side)
        {
            Color color = SideColors[side];
            var panel = new VerticalStackPanel
            {
                Width = SideWidth,
                Height = BodyHeight,
                Spacing = 12,
                Padding = new Thickness(12),
                Background = new FrameBrush(color * 0.16f, color, false)
            };
            panel.Widgets.Add(new Label
            {
                Text = SideNames[side],
                Font = TextManager.GetFont(20),
                TextColor = color,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            this.slotPanels[side] = new VerticalStackPanel
            {
                Spacing = 10
            };
            panel.Widgets.Add(this.slotPanels[side]);
            this.RefreshSlots(side);
            return panel;
        }

        private Widget BuildCenter()
        {
            var center = new VerticalStackPanel
            {
                Width = GridWidth
            };
            this.scenarioLabel = new Label
            {
                Font = TextManager.GetFont(17),
                TextColor = MyraSkin.TextTitle,
                Height = CenterHeaderHeight,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            };
            center.Widgets.Add(this.scenarioLabel);

            this.gridHost = new VerticalStackPanel
            {
                Spacing = CellSpacing,
                Width = GridWidth,
                Height = GridHeight
            };
            center.Widgets.Add(this.gridHost);
            return center;
        }

        private static Widget CreatePortrait(Person person)
        {
            var frame = new Panel
            {
                Width = PortraitWidth,
                Height = PortraitHeight,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidBrush(CardColor)
            };
            frame.Widgets.Add(new Image
            {
                Renderable = new DiplomacyDialog.PortraitImage(person, PortraitSize.Medium, PortraitWidth, PortraitHeight)
            });
            return frame;
        }

        private void BuildCells(List<Person> persons)
        {
            this.built = true;
            this.gridHost.Widgets.Clear();
            this.cells.Clear();

            for (int row = 0; row < Rows; row++)
            {
                var line = new HorizontalStackPanel
                {
                    Spacing = CellSpacing
                };
                for (int col = 0; col < Columns; col++)
                {
                    int index = row * Columns + col;
                    if (index >= persons.Count)
                    {
                        break;
                    }
                    Cell cell = this.CreateCell(persons[index]);
                    this.cells.Add(cell);
                    line.Widgets.Add(cell.Frame);
                }
                this.gridHost.Widgets.Add(line);
            }

            this.scenarioLabel.Text = "剧本：" + this.scenario.Title;
            this.SetRandomEnabled(this.cells.Count >= 2);
            this.hintLabel.Text = this.cells.Count >= 2 ? "点“随机”为双方挑选武将" : "这个剧本里能单挑的武将不够";
        }

        private Cell CreateCell(Person person)
        {
            var frame = new Panel
            {
                Width = CellWidth,
                Height = CellHeight,
                Background = new SolidBrush(CardColor),
                Border = new SolidBrush(LineColor),
                BorderThickness = new Thickness(2)
            };
            var content = new VerticalStackPanel
            {
                Spacing = 2,
                Padding = new Thickness(0, 4, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            content.Widgets.Add(CreatePortrait(person));
            content.Widgets.Add(new Label
            {
                Text = person.Name,
                Font = TextManager.GetFont(14),
                TextColor = MyraSkin.TextPrimary,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            frame.Widgets.Add(content);

            // 已被抽走的格子压暗
            var shade = new Panel
            {
                Background = ShadeBrush,
                Visible = false
            };
            frame.Widgets.Add(shade);

            return new Cell() { Person = person, Frame = frame, Shade = shade };
        }

        private void RefreshCells()
        {
            for (int i = 0; i < this.cells.Count; i++)
            {
                Cell cell = this.cells[i];
                Color border = i == this.cursor ? MyraSkin.TextHighlight : cell.Side >= 0 ? SideColors[cell.Side] : LineColor;
                cell.Frame.Border = new SolidBrush(border);
                cell.Shade.Visible = cell.Side >= 0 && i != this.cursor;
            }
        }

        /// <summary>
        /// 阵营栏的三个位置：抽到的武将依次排下来，没抽到的位置是空框。抽完后第一个是主将
        /// </summary>
        private void RefreshSlots(int side)
        {
            VerticalStackPanel slots = this.slotPanels[side];
            slots.Widgets.Clear();
            Color color = SideColors[side];

            for (int i = 0; i < MaxPerSide; i++)
            {
                Person person = i < this.teams[side].Count ? this.teams[side][i] : null;
                var slot = new Panel
                {
                    Height = SlotHeight,
                    Padding = new Thickness(8),
                    Background = new FrameBrush(person == null ? Color.Transparent : CardColor, person == null ? LineColor : color, person == null)
                };

                if (person == null)
                {
                    slot.Widgets.Add(new Label
                    {
                        Text = "？",
                        Font = TextManager.GetFont(24),
                        TextColor = MyraSkin.TextDisabled,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    });
                    slots.Widgets.Add(slot);
                    continue;
                }

                var row = new HorizontalStackPanel
                {
                    Spacing = 10,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Widget portrait = CreatePortrait(person);
                portrait.VerticalAlignment = VerticalAlignment.Center;
                row.Widgets.Add(portrait);

                var info = new VerticalStackPanel
                {
                    Spacing = 4,
                    VerticalAlignment = VerticalAlignment.Center
                };
                info.Widgets.Add(CreateLabel(person.Name, 18, MyraSkin.TextPrimary));
                info.Widgets.Add(CreateLabel("武力 " + person.NormalStrength, 14, MyraSkin.TextSecondary));
                if (i == 0 && !this.rolling)
                {
                    info.Widgets.Add(CreateTag("主将", MyraSkin.TextHighlight));
                }
                row.Widgets.Add(info);
                slot.Widgets.Add(row);
                slots.Widgets.Add(slot);
            }
        }

        private void SetRandomEnabled(bool enabled)
        {
            this.randomButton.Enabled = enabled;
            this.randomLabel.TextColor = enabled ? MyraSkin.TextPrimary : MyraSkin.TextSecondary;
        }

        #endregion

        #region 读取剧本

        // 读好的武将池放在这里，每次打开只从里面重新抽28人，不再读剧本。
        // 主菜单空闲时就在后台预读，点单挑时多半已经读好
        private static readonly object poolLock = new object();
        private static readonly Random poolRandom = new Random();
        private static List<Scenario> scenarioList;
        private static Scenario poolScenario;
        private static List<Person> pool;
        private static bool poolUsed;  // 随机剧本时用过一次就换下一个剧本
        private static Task loadTask;
        private static Scenario loadingScenario;
        private static string failedName;
        private static int generation;  // 开始游戏时清掉缓存，之前没读完的结果作废

        private static List<Scenario> ScenarioList
        {
            get
            {
                if (scenarioList == null)
                {
                    try
                    {
                        scenarioList = SimpleSerializer.DeserializeJsonFile<List<Scenario>>(Platform.Current.GetMODFile(@"Content\Data\Scenario\Scenarios.json"), false);
                    }
                    catch (Exception ex)
                    {
                        WebTools.TakeWarnMsg("读取剧本列表失败", "DantiaoSelectDialog", ex);
                    }
                    scenarioList ??= new List<Scenario>();
                }
                return scenarioList;
            }
        }

        /// <summary>
        /// 全局设置里指定的单挑剧本，没指定或找不到时为null，即每次随机
        /// </summary>
        private static Scenario ConfiguredScenario
        {
            get
            {
                string name = Setting.Current.GlobalVariables?.DantiaoScenario?.Trim();
                return String.IsNullOrEmpty(name) ? null : ScenarioList.FirstOrDefault(s => s.Name == name || s.Title == name);
            }
        }

        // 池子里的武将能否给这次打开用：指定剧本时要是同一个，随机时要没用过
        private static bool PoolFits(Scenario configured)
        {
            return pool != null && (configured == null ? !poolUsed : poolScenario.Name == configured.Name);
        }

        /// <summary>
        /// 主菜单空闲时每帧调用：下次打开要用的剧本还没读就在后台读
        /// </summary>
        public static void Preload()
        {
            lock (poolLock)
            {
                if (loadTask != null && !loadTask.IsCompleted)
                {
                    return;
                }
                Scenario configured = ConfiguredScenario;
                if (PoolFits(configured))
                {
                    return;
                }
                Scenario target = configured ?? (ScenarioList.Count > 0 ? ScenarioList[poolRandom.Next(ScenarioList.Count)] : null);
                // 读失败过的不自动重试，等打开弹窗时再试
                if (target == null || target.Name == failedName)
                {
                    return;
                }
                StartLoad(target);
            }
        }

        /// <summary>
        /// 开始游戏时调用，放掉缓存的剧本
        /// </summary>
        public static void ClearCache()
        {
            lock (poolLock)
            {
                generation++;
                pool = null;
                poolScenario = null;
            }
        }

        // 调用时须持有poolLock
        private static void StartLoad(Scenario target)
        {
            int gen = generation;
            loadingScenario = target;
            loadTask = Task.Run(() =>
            {
                try
                {
                    while (CommonData.CurrentReady == false)
                    {
                        Platform.Sleep(100);
                    }

                    List<Person> persons;
                    // 读剧本会改Session.Current.Scenario，和开始游戏排队；轮到时已经在进游戏就不读了
                    lock (MainGameScreen.ScenarioLoadLock)
                    {
                        if (Session.MainGame.loadingScreen != null || Session.MainGame.mainGameScreen != null)
                        {
                            return;
                        }
                        GameScenario loaded = MainGameScreen.LoadScenarioData(target.Name, true);
                        // 有势力的武将才进池子，文官不单挑
                        persons = loaded.AllPersons.Values.Where(p => p.BelongedFaction != null && !p.IsCivil()).ToList();
                    }

                    lock (poolLock)
                    {
                        if (gen == generation)
                        {
                            poolScenario = target;
                            pool = persons;
                            poolUsed = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    WebTools.TakeWarnMsg("读取剧本失败:" + target.Name, "DantiaoSelectDialog", ex);
                    lock (poolLock)
                    {
                        failedName = target.Name;
                    }
                }
            });
        }

        private void StartLoading()
        {
            lock (poolLock)
            {
                Scenario configured = ConfiguredScenario;
                if (PoolFits(configured))
                {
                    this.scenario = poolScenario;
                }
                else if (loadTask != null && !loadTask.IsCompleted && (configured == null || loadingScenario.Name == configured.Name))
                {
                    // 正在预读，等它读完
                    this.scenario = loadingScenario;
                }
                else
                {
                    this.scenario = configured ?? (ScenarioList.Count > 0 ? ScenarioList[this.rng.Next(ScenarioList.Count)] : null);
                    if (this.scenario != null)
                    {
                        failedName = null;
                        StartLoad(this.scenario);
                    }
                }
            }

            if (this.scenario == null)
            {
                this.scenarioLabel.Text = "没有可用的剧本";
                return;
            }
            this.scenarioLabel.Text = "正在读取剧本：" + this.scenario.Title + "……";
            this.TryTakePool();
        }

        private const int DefaultMinStrength = 70;

        /// <summary>
        /// 展示的武将武力下限，取全局设置，没设时按70
        /// </summary>
        private static int MinStrength
        {
            get
            {
                int value = Setting.Current.GlobalVariables?.DantiaoMinStrength ?? 0;
                return value > 0 ? value : DefaultMinStrength;
            }
        }

        // 从武力达到下限的武将里随机抽28人；不够28人时用其余武力最高的补齐，再打乱顺序
        private List<Person> PickFromPool(List<Person> all)
        {
            int count = Rows * Columns;
            int min = MinStrength;
            List<Person> picked = all.Where(p => p.NormalStrength >= min).OrderBy(p => this.rng.Next()).Take(count).ToList();
            if (picked.Count < count)
            {
                picked.AddRange(all.Where(p => p.NormalStrength < min).OrderByDescending(p => p.NormalStrength).Take(count - picked.Count));
            }
            return picked.OrderBy(p => this.rng.Next()).ToList();
        }

        // 池子读好了就随机抽28人建格子；读失败了就提示
        private void TryTakePool()
        {
            List<Person> persons = null;
            lock (poolLock)
            {
                if (pool != null && poolScenario.Name == this.scenario.Name)
                {
                    persons = this.PickFromPool(pool);
                    poolUsed = true;
                }
                else if (failedName == this.scenario.Name)
                {
                    this.scenarioLabel.Text = "读取剧本失败：" + this.scenario.Title;
                    this.loadFailed = true;
                }
            }
            if (persons != null)
            {
                this.BuildCells(persons);
            }
        }

        #endregion

        #region 抽签

        /// <summary>
        /// 主菜单每帧调用：读完剧本时建格子，抽签时推动选框
        /// </summary>
        public void Update(float seconds)
        {
            if (!this.built)
            {
                if (this.scenario != null && !this.loadFailed)
                {
                    this.TryTakePool();
                }
                return;
            }

            if (!this.rolling)
            {
                return;
            }

            this.hopTimer += seconds;
            if (this.hopTimer < this.hopInterval)
            {
                return;
            }
            this.hopTimer = 0f;

            if (this.hopsLeft > 0)
            {
                this.hopsLeft--;
                this.cursor = this.RandomFreeCell(this.cursor);
                this.hopInterval *= HopSlowdown;
                Platform.Current.PlayEffect(@"Content\Sound\Click");
            }
            else
            {
                var (side, index) = this.pending.Dequeue();
                this.cursor = index;
                this.cells[index].Side = side;
                this.teams[side].Add(this.cells[index].Person);
                this.RefreshSlots(side);
                Platform.Current.PlayEffect(@"Content\Sound\Select");
                this.NextPick();
                // 落定后停一下再抽下一个
                this.hopTimer = -PauseAfterPick;
            }
            this.RefreshCells();
        }

        // 选框跳到一个还没被抽走的格子，不停在原地
        private int RandomFreeCell(int current)
        {
            var free = Enumerable.Range(0, this.cells.Count).Where(i => this.cells[i].Side < 0 && i != current).ToList();
            return free.Count == 0 ? current : free[this.rng.Next(free.Count)];
        }

        /// <summary>
        /// 重新抽签：两边各抽1到3人，人数和人选都随机，左右轮流落定
        /// </summary>
        private void Roll()
        {
            if (this.rolling || this.cells.Count < 2)
            {
                return;
            }

            this.teams[0].Clear();
            this.teams[1].Clear();
            this.cells.ForEach(c => c.Side = -1);
            this.startButton.Visible = false;

            var (leftCount, rightCount) = this.RollCounts();
            leftCount = Math.Min(leftCount, this.cells.Count - 1);
            rightCount = Math.Min(rightCount, this.cells.Count - leftCount);
            List<int> picks = Enumerable.Range(0, this.cells.Count).OrderBy(i => this.rng.Next()).Take(leftCount + rightCount).ToList();

            this.pending.Clear();
            int side = 0;
            int next = 0;
            int[] remaining = { leftCount, rightCount };
            while (remaining[0] > 0 || remaining[1] > 0)
            {
                if (remaining[side] == 0)
                {
                    side = 1 - side;
                }
                this.pending.Enqueue((side, picks[next++]));
                remaining[side]--;
                side = 1 - side;
            }

            this.rolling = true;
            this.SetRandomEnabled(false);
            this.hintLabel.Text = "抽签中……";
            this.RefreshSlots(0);
            this.RefreshSlots(1);
            this.NextPick();
            this.hopTimer = 0f;
            this.RefreshCells();
        }

        // 双方人数的组合和权重。两边各自等概率抽1到3人时，有一边只有1人的占一半以上，
        // 这里按组合加权：多对多为主，一对多、一对一少些。不对等的组合再随机分给左右
        private static readonly (int Few, int Many, int Weight)[] CountWeights =
        {
            (1, 1, 10),
            (1, 2, 10),
            (1, 3, 10),
            (2, 2, 20),
            (2, 3, 30),
            (3, 3, 20)
        };

        private (int Left, int Right) RollCounts()
        {
            int roll = this.rng.Next(CountWeights.Sum(c => c.Weight));
            foreach (var c in CountWeights)
            {
                if ((roll -= c.Weight) < 0)
                {
                    return this.rng.Next(2) == 0 ? (c.Few, c.Many) : (c.Many, c.Few);
                }
            }
            return (1, 1);
        }

        private void NextPick()
        {
            if (this.pending.Count == 0)
            {
                this.Finish();
                return;
            }
            this.hopsLeft = this.rng.Next(MinHops, MaxHops + 1);
            this.hopInterval = FirstHopInterval;
        }

        // 抽完了：每方武力最高的排第一当主将
        private void Finish()
        {
            this.rolling = false;
            this.cursor = -1;
            for (int side = 0; side < 2; side++)
            {
                List<Person> sorted = this.teams[side].OrderByDescending(p => p.NormalStrength).ToList();
                this.teams[side].Clear();
                this.teams[side].AddRange(sorted);
                this.RefreshSlots(side);
            }
            this.startButton.Visible = true;
            this.SetRandomEnabled(true);
            this.hintLabel.Text = "点“开始”进入单挑，点“随机”重新挑选";
        }

        private void Start()
        {
            if (this.rolling || this.teams[0].Count == 0 || this.teams[1].Count == 0)
            {
                return;
            }
            var left = this.teams[0].ToList();
            var right = this.teams[1].ToList();
            Scenario chosen = this.scenario;
            // 等鼠标松开再进单挑，避免这次点击落到后面的界面上
            this.Close(() => this.onStart(chosen, left, right));
        }

        #endregion
    }
}
