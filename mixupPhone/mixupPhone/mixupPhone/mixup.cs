using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;



namespace mixupPhone
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class mixup : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //我定义的变量
        //音效
        SoundEffect clickvoice;
        SoundEffect rightvoice;
        Song backvoice;
        Song open;
          
        //游戏状态 开始 游戏中 还是结束
        enum GameState { Start, InGame, GameOver,Pause };
        GameState gameState = GameState.Start;
        //背景
        Texture2D background;
        Texture2D pauseback;
        Texture2D start;
        Texture2D endback;
        //对象
        Texture2D pause;
        Texture2D music;
        Texture2D texture;
        Texture2D pen;
        Texture2D target;
        Texture2D animal;
        Texture2D tryagain;
        
        
        Texture2D  box;
        SpriteFont scoreFont;
        SpriteFont Saying;
        Texture2D timeLine;
        MouseState mousePos = Mouse.GetState();
        string sayingwords;
        //对象位置
        Point penPos;
        Point[] squarePos = new Point[25];  //矩阵每一点的位置
        Point targetPos;
        Point animalPos = new Point(280, 640);
     
       


        //对象颜色
        Color OpenR;
        Color OpenG;
        Color OpenB;
        Color[] squarColor = new Color[25];
        Color targetColor ;
        Color musicColor = Color.White;
        Color tryagainColor=Color.White;
        //动画
        int animalSpeed;

       
        // 用来绘制的原色
        Color[] colorArray = new Color[3];//初始化只有3种颜色

        //用于碰撞测试鼠标和对象
        Rectangle[] squareRect = new Rectangle[25];

        //随着交互而变化的量
        int click;
    
        Color preColor;
        int updateMouse = 0;
        int pre;
        int score;
       
        //计时器们
        int timeLength;
        int timeCounter;
        int openCounter;

        public mixup()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
           
            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }




        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            tryagainColor = Color.White;
                           
            musicColor = Color.White;
            targetPos = new Point(390, 100);
            click = 0;
            score = 0;
            openCounter = 100;
            animalSpeed = 5;
            sayingwords = "Let's mix up!";
            timeCounter = 0;
            timeLength = 410;
            animalPos = new Point(280, 640);
            //初始化颜色数组
            colorArray[0] = new Color(221, 45, 35);
            colorArray[1] = new Color(47, 142, 217);
            colorArray[2] = new Color(59, 139, 39);
          

            //生成颜色矩阵

            int x = 10; //第一个方格的坐标
            int y = -466; //第一个方格的坐标

            for (int i = 0; i < 25; i++)
            {
                squarePos[i].X = x;
                squarePos[i].Y = y;
                if (i > 0)
                {
                    //给每个方格上颜色
                    do
                    {
                        squarColor[i] = randomColor();
                    } while (squarColor[i] == squarColor[i - 1]);
                }
               else
                    squarColor[i] = randomColor();
                x += 74; //方格的边长+空隙值
                if (x >= 370)
                {
                    x = 10;
                    y += 74;
                }

            }


           
            targetColor = targetcolor(); //使用targetcolor()颜色生成目标颜
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            clickvoice = Content.Load<SoundEffect>(@"audio/click");
            rightvoice = Content.Load<SoundEffect>(@"audio/right");
            backvoice = Content.Load<Song>(@"audio/background");
            open = Content.Load<Song>(@"audio/open");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            endback = Content.Load<Texture2D>(@"images/endback");
            start = Content.Load<Texture2D>(@"images/start");
            pauseback = Content.Load<Texture2D>(@"images/pauseback");
            pause = Content.Load<Texture2D>(@"images/pause");
            music = Content.Load<Texture2D>(@"images/music");
            texture = Content.Load<Texture2D>(@"images/square");
            tryagain = Content.Load<Texture2D>(@"images/tryagain");
            pen = Content.Load<Texture2D>(@"images/pen");
            target = Content.Load<Texture2D>(@"images/target");
            scoreFont = Content.Load<SpriteFont>(@"font/score");
            background = Content.Load<Texture2D>(@"images/back");
            timeLine = Content.Load<Texture2D>(@"images/timeline");
            animal = Content.Load<Texture2D>(@"images/animal");
            Saying = Content.Load<SpriteFont>(@"font/Sayingfont");
            box = Content.Load<Texture2D>(@"images/box");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            TouchCollection touchCollection = TouchPanel.GetState();
            animalPos.Y+= animalSpeed;
            if ((animalPos.Y > 660)||(animalPos.Y<640))
                animalSpeed *= -1;
            // TODO: Add your update logic here
            switch (gameState)
            {
                case GameState.Start:
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                        this.Exit();
                    if (timeCounter < 100)
                    {
                        timeCounter++;
                        if (timeCounter %10==5)
                            OpenR = randomColor();
                        if (timeCounter % 10 == 0)
                            OpenG = randomColor();
                        if (timeCounter % 10 == 7)
                            OpenB = randomColor();

                    }
                    else
                    {
                        timeCounter = 0;
                    }
                    if (MediaPlayer.State != MediaState.Playing)
                        MediaPlayer.Play(open);
                  //if (Keyboard.GetState().GetPressedKeys().Length > 0)
                  
                   /*  if (left == ButtonState.Pressed)
                    {
                        gameState = GameState.InGame;
                        click = 0;
                        Initialize();
                    }*/
                    foreach (TouchLocation tl in touchCollection)
                    {
                        if (tl.State == TouchLocationState.Pressed)
                        {
                            MediaPlayer.Pause();
                            Initialize();
                            gameState = GameState.InGame;
                            MediaPlayer.Play(backvoice);
                        }

                    }
                    break;
                case GameState.Pause:
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                        this.Exit();
                  
                    foreach (TouchLocation tl in touchCollection)
                    {
                        if (tl.State == TouchLocationState.Pressed)
                        {
                            Rectangle Pos = new Rectangle((int)(tl.Position.X-10),(int)(tl.Position.Y-10),20,20);
                            Rectangle Continue = new Rectangle(76,403,296,46);
                            Rectangle Home = new Rectangle(324, 665, 45, 45);
                            if(Continue.Intersects(Pos))
                               gameState = GameState.InGame;
                            if (Home.Intersects(Pos))
                            {
                                if (MediaPlayer.State == MediaState.Playing)
                                    MediaPlayer.Pause();
                                gameState = GameState.Start;
                            }
                           
                        }


                    }
                    break;
                case GameState.InGame:
                    // Allows the game to exit
                   
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                        this.Exit();
                    if (openCounter >= 0)
                    {
                        openCounter -= 10;
                        
                        for (int i = 0; i < 25; i++)
                        {
                           
                            squarePos[i].Y += 50;
                        }
                        
                    }
                    else
                    {
                        //生成目标颜色的方格
                        for (int i = 0; i < 25; i++)
                        {
                            squareRect[i] = new Rectangle((int)squarePos[i].X, squarePos[i].Y, 70, 70);
                        }
                        //时间轴更新
                        timeCounter += 1;
                        if (timeCounter == 30)
                        {
                            timeLength -= 8;
                            timeCounter = 0;
                        }
                        // TODO: Add your update logic here

                        if (targetPos.Y <= 380)
                        {
                            targetPos.Y += 6;

                        }
                        //   penPos = new Point(mousePos.X, mousePos.Y);
                        foreach (TouchLocation tl in touchCollection)
                        {
                            if (tl.State == TouchLocationState.Released)

                                updateMouse = 0;

                            if (tl.State == TouchLocationState.Pressed)
                            {
                                if (updateMouse == 0)
                                {
                                    updateMouse = 1;
                                    int index = Collide(tl);

                                    if (index != 27)  //i=27 表示点击了鼠标，但没和方块碰撞
                                    {

                                        if ((index == 29) || (index == 30))
                                        {
                                            if (index == 30) //点击pause
                                            {
                                                gameState = GameState.Pause;
                                            }
                                            else
                                            {
                                                if (MediaPlayer.State == MediaState.Playing)
                                                {
                                                    MediaPlayer.Pause();
                                                    musicColor = Color.Violet;
                                                }
                                                else if (MediaPlayer.State != MediaState.Playing)
                                                {
                                                    MediaPlayer.Play(backvoice);
                                                    musicColor = Color.White;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (click == 0) //选择第一个方块
                                            {
                                                clickvoice.Play();
                                                sayingwords = "Let's mix up!";
                                                click = 1;
                                                preColor = squarColor[index];//保存第一块方块的信息
                                                squarColor[index].A += 100;
                                                pre = index;
                                            }
                                            else if (click == 1)//seconde click
                                            {
                                                click = 0;
                                                clickvoice.Play();
                                                squarColor[index].A += 100;
                                                if (targetColor.Equals(new Color(squarColor[index].R + preColor.R, squarColor[index].G + preColor.G, squarColor[index].B + preColor.B)))
                                                {


                                                    squarColor[index] = targetColor;
                                                    squarColor[pre] = targetColor;


                                                    targetPos.Y = 100;

                                                    targetColor = targetcolor();
                                                    timeLength += 20;
                                                    score += 1;
                                                    sayingwords = "good job!";
                                                    rightvoice.Play();
                                                }
                                                else
                                                {
                                                    if (!squarePos[pre].Equals(squarePos[index]))
                                                        timeLength -= 40;
                                                    squarColor[pre].A -= 100;
                                                    squarColor[index].A -= 100;
                                                    sayingwords = "Opps! try again.";
                                                }

                                            }
                                        }
                                    }

                                }

                            }
                        }

                        if (timeLength <= 50)
                        {
                            for (int i = 0; i < 25; i++)
                            {


                                squarColor[i].A += 100; ;
                            }

                        }
                        if (timeLength <= 0)
                        {
                            MediaPlayer.Pause();
                            gameState = GameState.GameOver;
                            click = 0;
                        }
                    }
                    break;
                    case GameState.GameOver:
                    
                    if (MediaPlayer.State != MediaState.Playing)
                        MediaPlayer.Play(open);
                    Rectangle tryRect = new Rectangle(335, 721, 114, 52);
                    
                    foreach(TouchLocation tl in touchCollection)
                    {
                        Rectangle tlRect = new Rectangle((int)tl.Position.X - 10,(int) tl.Position.Y - 10, 20, 20);
                         if (tryRect.Intersects(tlRect))
                            tryagainColor=Color.Red;
                        else
                            tryagainColor=Color.White;
                           
                        if (tl.State ==TouchLocationState.Pressed)
                        {
                            if (tryRect.Intersects(tlRect))
                                 click = 1;
                            //    gameState = GameState.Start;

                        }
                        if (tl.State == TouchLocationState.Released)
                        {
                            if (click == 1)
                            {
                                gameState = GameState.Start;
                                click = 0;
                            }
                        }
                    }
                    
                    
                    break;
            }//switch over
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(22, 19, 70));
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            string text = "Touch the panel to begin";
            switch (gameState)
            {
                case GameState.Start:
                    //GraphicsDevice.Clear(Color.AliceBlue);
                  
                    spriteBatch.Draw(start, new Vector2(0, 0), Color.White);
                    spriteBatch.Draw(target, new Rectangle(220, 217, 236, 160), OpenR);
                    spriteBatch.Draw(target, new Rectangle(338, 400, 126, 142), OpenG);
                    spriteBatch.Draw(target, new Rectangle(223, 560, 237, 168), OpenB);
                    break;
                case GameState.Pause:
                    spriteBatch.Draw(pauseback,new Vector2(0,0),Color.White);
                    break;
                case GameState.InGame:
                

                    //spriteBatch.Draw(background,new Rectangle(0,0,Window.ClientBounds.Width,Window.ClientBounds.Height),null,Color.Orange,0,Vector2.Zero,SpriteEffects.None,0);
                    for (int i = 0; i < 25; i++)
                    {
                        //random = randomColor();
                        spriteBatch.Draw(texture, new Vector2(squarePos[i].X, squarePos[i].Y), squarColor[i]);
                    }

                 //   spriteBatch.Draw(pen, new Vector2(penPos.X, penPos.Y), Color.White);
                    spriteBatch.Draw(target, new Vector2(targetPos.X, targetPos.Y), targetColor);
                    spriteBatch.DrawString(scoreFont, "Score: " + score, new Vector2(60, 510), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                    spriteBatch.Draw(timeLine, new Rectangle(50, 20, 410, 25), Color.White);
                    spriteBatch.Draw(timeLine, new Rectangle(50, 20, timeLength, 25), Color.DarkRed);
                    spriteBatch.Draw(animal, new Vector2(animalPos.X,animalPos.Y), Color.Orange);
                    spriteBatch.DrawString(Saying, sayingwords, new Vector2(60,580), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                    spriteBatch.Draw(box, new Vector2(390, 100), Color.Orange);
                    spriteBatch.Draw(music, new Vector2(30, 700), musicColor);
                    spriteBatch.Draw(pause, new Vector2(10, 20), Color.White);
                    break;
                case GameState.GameOver:
                 //   GraphicsDevice.Clear(Color.AliceBlue);


                    spriteBatch.Draw(endback, new Vector2(0, 0), Color.White);
                    spriteBatch.Draw(tryagain, new Vector2(335,721), tryagainColor);
                    
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        //用于生成随机颜色的函数
        public Color randomColor()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            /*  byte r = (byte)random.Next(0, 255);
              byte g = (byte)random.Next(0, 255);
              byte b = (byte)random.Next(0, 255);*/
            int temp = random.Next(0, 3);
            return colorArray[temp];
        }

        //生成目标方块的颜色
        public Color targetcolor()
        {
            Color target;
            Random random = new Random(DateTime.Now.Millisecond);
            int r = random.Next(0, 3);
            int t;
            do
            {
                t = random.Next(0, 25);
            } while (squarColor[t].Equals(squarColor[r]));
            target = new Color(squarColor[t].R + squarColor[r].R, squarColor[t].G + squarColor[r].G, squarColor[t].B + squarColor[r].B);
            return target;
        }
        //检查鼠标与哪一个方块碰撞了
        public int Collide(TouchLocation tl)
        {
            Rectangle mouseRect = new Rectangle((int)tl.Position.X-10,(int)tl.Position.Y-10, 20, 20);
            for (int i = 0; i < 25; i++)
                if (squareRect[i].Intersects(mouseRect))
                {
                    return i;
                }
            Rectangle musicPos = new Rectangle(30, 700, 40, 40);
            if(musicPos.Intersects(mouseRect))
                  return 29;
            Rectangle pausePos = new Rectangle(10, 20, 40, 40);
            if (pausePos.Intersects(mouseRect))
                return 30;
            return 27;
        }

    }
}
