﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoPong.Effects;

namespace MonoPong.Player
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Paddle
    {
        private int _paddleWidth = 21;
        private int _paddleHeight = 101;
        private readonly List<Keys> _upKeys = new List<Keys>();
        private readonly List<Keys> _downKeys = new List<Keys>();
        private readonly List<Keys> _boostKeys = new List<Keys>();
        private Vector2 _paddlePosition;
        private float _gameSpeed = 1f;

        private Texture2D _paddleTexture;
        private Boost _boost1;
        private Boost _boost2;
        private int _boost1Width = 12;
        private int _boost1Height = 50;
        private int _boost2Width = 10;
        private int _boost2Height = 26;
        private int _boostState;

        public Paddle(int x, int y)
        {
            _paddlePosition = new Vector2(x, y);
            _boost1 = new Boost(x + _paddleWidth + 4,y + ((_paddleHeight - _boost1Height) / 2), _boost1Width, _boost1Height, 2f);
            _boost2 = new Boost(x + _paddleWidth + 8 + _boost1Width, y + ((_paddleHeight - _boost2Height) / 2), _boost2Width, _boost2Height, 3f);
            _boostState = 0;
        }

        public float GameSpeed
        {
            get { return _gameSpeed; }
            set { _gameSpeed = value; }
        }

        public void AddUpKeys(Keys key)
        {
            _upKeys.Add(key);
        }

        public void AddDownKeys(Keys key)
        {
            _downKeys.Add(key);
        }
        public void AddBoostKeys(Keys key)
        {
            _boostKeys.Add(key);
        }

        public float GetX()
        {
            return _paddlePosition.X;
        }

        public float GetY()
        {
            return _paddlePosition.Y;
        }

        private Vector2 GetPosition()
        {
            return _paddlePosition;
        }

        private void MovePaddle(float ySpeed)
        {
            if (_paddlePosition.Y - ySpeed <= 0 && ySpeed < 0)
                ySpeed = 0 - _paddlePosition.Y;
            if (_paddlePosition.Y + ySpeed >= 380 && ySpeed > 0)
                ySpeed = 380 - _paddlePosition.Y;

            _paddlePosition.Y += ySpeed;
            _boost1.Move(0, ySpeed);
            _boost2.Move(0, ySpeed);
        }

        public void HandleKeystrokes()
        {
            var upPressed = _upKeys.Any(x => Keyboard.GetState().IsKeyDown(x));
            var downPressed = _downKeys.Any(x => Keyboard.GetState().IsKeyDown(x));
            var boostPressed = _boostKeys.Any(x => Keyboard.GetState().IsKeyDown(x));

            if (upPressed)
            {
                MovePaddle(-1 * GameSpeed);
            }

            if (downPressed)
            {
                MovePaddle(1 * GameSpeed);
            }

            if (boostPressed && _boostState == 0)
            {
                _boostState = 1;
            }
        }

        public void SetColor(Color color)
        {
            Color[] paddleColorData = new Color[_paddleWidth * _paddleHeight];
            for (int i = 0; i < _paddleWidth * _paddleHeight; i++)
            {
                paddleColorData[i] = color;
            }
            _paddleTexture.SetData(paddleColorData);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_paddleTexture, GetPosition(), Color.White);
            if (_boost1.Status())
            {
                _boost1.Draw(spriteBatch);
            }

            if (_boost2.Status())
            {
                _boost2.Draw(spriteBatch);
            }
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            //Set up Paddle
            _paddleTexture = new Texture2D(graphicsDevice, _paddleWidth, _paddleHeight);
            SetColor(Color.White);

            //Set up Boost 1
            _boost1.BoostTexture = new Texture2D(graphicsDevice, _boost1Width, _boost1Height);
            _boost1.SetColor(Color.Blue);

            //Set up Boost 2
            _boost2.BoostTexture = new Texture2D(graphicsDevice, _boost2Width, _boost2Height);
            _boost2.SetColor(Color.CornflowerBlue);

        }

        public void Initialize()
        {

        }

        public void ManageBoost()
        {
            //Increment Cooldown State
            if (_boostState < 0)
                _boostState++;
            //Boost1 Active
            else if (_boostState == 1)
            {
                _boost1.On();
                _boostState++;
            }
            else if (_boostState > 1 && _boostState < 10)
                _boostState++;
            //Boost2 Active
            else if (_boostState == 10)
            {
                _boost1.Off();
                _boost2.On();
                _boostState++;
            }
            else if (_boostState > 10 && _boostState < 20)
                _boostState++;
            //Boost on Cooldown
            else if (_boostState >= 20)
            {
                _boost2.Off();
                _boostState = -100;
            }
        }
    }
}