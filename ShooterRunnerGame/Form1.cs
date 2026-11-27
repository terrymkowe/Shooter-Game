using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShooterRunnerGame
{
    public partial class Form1 : Form
    {
        private const int PlayerSpeed = 5;
        private const int BulletSpeed = 10;
        private const int EnemySpeed = 3;
        private const int EnemySpawnInterval = 1000; // in milliseconds

        private PictureBox player;
        private Timer gameLoop;
        private List<PictureBox> bullets;
        private List<PictureBox> enemies;
        private Random rand;

        private Label gameOverLabel;
        private Label restartLabel;
        private Label exitLabel;

        private int score = 0;
        private Label scoreLabel;
       
        private int hitCount = 0; // Track the number of enemy hits
        private void RestartLabel_Click(object sender, EventArgs e)
        {
            // Remove labels
            Controls.Remove(gameOverLabel);
            Controls.Remove(restartLabel);
            Controls.Remove(exitLabel);

            score = 0;
            UpdateScoreLabel(); // Update the score label to display the reset score


            // Reset the game
            InitializeGame();
        }

        private void ExitLabel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            //event handlers for key down and key up events
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.KeyDown(e.KeyCode);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.KeyUp(e.KeyCode);
        }
        private void InitializeGame()
        {
            rand = new Random();
            bullets = new List<PictureBox>();
            enemies = new List<PictureBox>();

            player = new PictureBox();
            player.BackColor = Color.Blue;
            player.Size = new Size(50, 50);
            player.Location = new Point(ClientSize.Width / 2 - player.Width / 2, ClientSize.Height - player.Height - 10);
            Controls.Add(player);

            gameLoop = new Timer();
            gameLoop.Interval = 16; // 60 FPS
            gameLoop.Tick += GameLoop_Tick;
            gameLoop.Start();

            SpawnEnemy();

            // Create score label
            scoreLabel = new Label();
            scoreLabel.Text = "Score: 0";
            scoreLabel.AutoSize = true;
            scoreLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            scoreLabel.ForeColor = Color.Black;
            scoreLabel.Location = new Point(10, 10); // Adjust the position as needed
            Controls.Add(scoreLabel);
        }

        private void GameLoop_Tick(object sender, EventArgs e)
        {
            MovePlayer();
            MoveBullets();
            MoveEnemies();
            CheckCollisions();
            CheckPlayerDeath(); // check if the player has died
        }

        private void CheckPlayerDeath()
        {
            Rectangle playerBounds = player.Bounds;

            foreach (PictureBox enemy in enemies)
            {
                if (playerBounds.IntersectsWith(enemy.Bounds))
                {
                    // Player is hit by an enemy, perform actions for player death
                    EndGame(); // Example: Call a method to end the game
                    break;
                }
            }
        }

        private void EndGame()
        {
            gameLoop.Stop(); // Stop the game loop

            // Clear bullets and enemies from the screen
            foreach (var bullet in bullets)
            {
                Controls.Remove(bullet);
            }
            bullets.Clear();

            foreach (var enemy in enemies)
            {
                Controls.Remove(enemy);
            }
            enemies.Clear();

            // Remove the player cube
            Controls.Remove(player);

            // Show game over label
            gameOverLabel = new Label();
            gameOverLabel.Text = "Game Over!";
            gameOverLabel.AutoSize = true;
            gameOverLabel.Font = new Font("Arial", 20, FontStyle.Bold);
            gameOverLabel.ForeColor = Color.Red;
            gameOverLabel.Location = new Point(ClientSize.Width / 2 - gameOverLabel.Width / 2, ClientSize.Height / 2 - gameOverLabel.Height / 2);
            Controls.Add(gameOverLabel);

            // Show restart label
            restartLabel = new Label();
            restartLabel.Text = "Restart Game";
            restartLabel.AutoSize = true;
            restartLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            restartLabel.ForeColor = Color.Blue;
            restartLabel.Location = new Point(ClientSize.Width / 2 - restartLabel.Width / 2, gameOverLabel.Bottom + 20); // Position below the game over label
            restartLabel.Cursor = Cursors.Hand;
            restartLabel.Click += RestartLabel_Click;
            Controls.Add(restartLabel);

            // Show exit label
            exitLabel = new Label();
            exitLabel.Text = "Exit Game";
            exitLabel.AutoSize = true;
            exitLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            exitLabel.ForeColor = Color.Blue;
            exitLabel.Location = new Point(ClientSize.Width / 2 - exitLabel.Width / 2, restartLabel.Bottom + 10);
            exitLabel.Cursor = Cursors.Hand;
            exitLabel.Click += ExitLabel_Click;
            Controls.Add(exitLabel);
        }
      

       
        private void MovePlayer()
        {
            if (Input.IsKeyDown(Keys.Left))
            {
                if (player.Left > 0)
                    player.Left -= PlayerSpeed;
            }

            if (Input.IsKeyDown(Keys.Right))
            {
                if (player.Right < ClientSize.Width)
                    player.Left += PlayerSpeed;
            }

            if (Input.IsKeyDown(Keys.Space))
            {
                FireBullet();
            }
        }

        private void MoveBullets()
        {
            foreach (PictureBox bullet in bullets)
            {
                bullet.Top -= BulletSpeed;
                if (bullet.Top + bullet.Height < 0)
                {
                    Controls.Remove(bullet);
                    bullets.Remove(bullet);
                    break;
                }
            }
        }

        private void MoveEnemies()
        {
            foreach (PictureBox enemy in enemies)
            {
                enemy.Top += EnemySpeed;
                if (enemy.Top > ClientSize.Height)
                {
                    Controls.Remove(enemy);
                    enemies.Remove(enemy);
                    SpawnEnemy();
                    break;
                }
            }
        }

        private void FireBullet()
        {
            PictureBox bullet = new PictureBox();
            bullet.BackColor = Color.Red;
            bullet.Size = new Size(5, 20);
            bullet.Location = new Point(player.Left + player.Width / 2 - bullet.Width / 2, player.Top - bullet.Height);
            Controls.Add(bullet);
            bullets.Add(bullet);
        }

        private void SpawnEnemy()
        {
            PictureBox enemy = new PictureBox();
            enemy.BackColor = Color.Green;
            enemy.Size = new Size(30, 30);
            enemy.Location = new Point(rand.Next(ClientSize.Width - enemy.Width), -enemy.Height);
            Controls.Add(enemy);
            enemies.Add(enemy);
        }
        private void CheckCollisions()
        {
            List<PictureBox> enemiesToRemove = new List<PictureBox>();
            List<PictureBox> bulletsToRemove = new List<PictureBox>();

            foreach (PictureBox bullet in bullets)
            {
                foreach (PictureBox enemy in enemies)
                {
                    if (bullet.Bounds.IntersectsWith(enemy.Bounds))
                    {
                        bulletsToRemove.Add(bullet);
                        enemiesToRemove.Add(enemy);
                        hitCount++; // Increase hit count
                        if (hitCount % 10 == 0) // Check if hitCount is a multiple of 10
                        {
                          //  IncreaseEnemySpeed(); // Increase enemy speed after every 10 hits
                        }
                        score += 1; // Increase score by 1 for each enemy shot
                        UpdateScoreLabel();
                    }
                }
            }

            // Remove enemies and bullets that need to be removed
            foreach (PictureBox enemy in enemiesToRemove)
            {
                Controls.Remove(enemy);
                enemies.Remove(enemy);
            }

            foreach (PictureBox bullet in bulletsToRemove)
            {
                Controls.Remove(bullet);
                bullets.Remove(bullet);
            }

            // Respawn enemies for each one removed
            foreach (PictureBox enemy in enemiesToRemove)
            {
                SpawnEnemy();
            }
        }

  
        private void UpdateScoreLabel()
        {
            scoreLabel.Text = "Score: " + score.ToString();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    public static class Input
    {
        private static HashSet<Keys> keys = new HashSet<Keys>();

        public static void KeyDown(Keys key)
        {
            keys.Add(key);
        }

        public static void KeyUp(Keys key)
        {
            keys.Remove(key);
        }

        public static bool IsKeyDown(Keys key)
        {
            return keys.Contains(key);
        }
    }

    public class GameForm : Form
    {
        public GameForm()
        {
            KeyDown += (sender, e) => Input.KeyDown(e.KeyCode);
            KeyUp += (sender, e) => Input.KeyUp(e.KeyCode);
        }
    }
}