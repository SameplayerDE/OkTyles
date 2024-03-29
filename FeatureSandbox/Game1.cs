﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FeatureSandbox;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;
    private List<Texture2D> _texture = new();

    private Vector3 _cameraPosition = new Vector3(0, 0, 5f);
    private Vector3 _cameraTarget = Vector3.Zero;
    private Vector3 _cameraUpVector = Vector3.UnitY;

    private Matrix _viewMatrix;
    private Matrix _projectionMatrix;
    private Matrix _worldMatrix;
    private BasicEffect _basicEffect;

    private VertexPositionTexture[] _vertices;
    private short[] _indices;

    private float _offset = 0.1f; // Initialer Offset-Wert
    private float _targetOffset = 0.1f; // Ziel-Offset-Wert

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);


        // Create the projection matrix
        float aspectRatio = GraphicsDevice.Viewport.AspectRatio;
        float fieldOfView = MathHelper.ToRadians(45);
        float nearClipPlane = 0.1f;
        float farClipPlane = 1000f;
        _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);

        // Initialize world matrix
        _worldMatrix = Matrix.Identity;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _texture.Add(Content.Load<Texture2D>("Layer_0"));
        _texture.Add(Content.Load<Texture2D>("Layer_1"));
        _texture.Add(Content.Load<Texture2D>("Layer_2"));

        // Setup basic effect
        _basicEffect = new BasicEffect(GraphicsDevice);
        _basicEffect.TextureEnabled = true;

        float aspectRatio = (float)_texture[0].Width / _texture[0].Height;

        // Create vertices for a quad
        _vertices = new VertexPositionTexture[]
        {
            new VertexPositionTexture(new Vector3(-aspectRatio, 1, 0), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(aspectRatio, 1, 0), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(-aspectRatio, -1, 0), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(aspectRatio, -1, 0), new Vector2(1, 1)),
        };

        // Create indices
        _indices = new short[] { 0, 1, 2, 1, 3, 2 };
    }


    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            _cameraPosition.X = MathHelper.Lerp(_cameraPosition.X, -2.5f, 0.1f);
            _cameraPosition.Z = MathHelper.Lerp(_cameraPosition.Z, 5f, 0.1f);
            _targetOffset = 0.5f;
        }
        else
        {
            _cameraPosition.X = MathHelper.Lerp(_cameraPosition.X, 0f, 0.1f);
            _cameraPosition.Z = MathHelper.Lerp(_cameraPosition.Z, 5f, 0.1f);
            _targetOffset = 0.001f;
        }

        _offset = MathHelper.Lerp(_offset, _targetOffset, 0.1f);
        _viewMatrix = Matrix.CreateLookAt(_cameraPosition, _cameraTarget, _cameraUpVector);


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
            GraphicsDevice.Viewport.AspectRatio, 0.1f, 100f);
        
        GraphicsDevice.BlendState = BlendState.AlphaBlend;
        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        
        _basicEffect.View = _viewMatrix;
        _basicEffect.World = _worldMatrix;

        // Zeichne das Quad viermal mit unterschiedlichen Z-Positionen
        for (int i = 0; i < _texture.Count; i++)
        {
            _basicEffect.World = Matrix.CreateTranslation(0, 0, i * _offset);
            _basicEffect.Texture = _texture[i];

            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, 4, _indices, 0, 2);
            }
        }

        base.Draw(gameTime);
    }
}