using System;
using System.Collections.Generic;
using OkTyles.Core;
using Bembelbuben.Core;
using Bembelbuben.Core.Input;
using Bembelbuben.Core.UserInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OkTyles.Core.Commands;

namespace OkTyles;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private UserInterfaceRenderer _uiRenderer;

    private Texture2D _pixel;
    private SpriteFont _font;

    private Camera _camera;

    private float _delta;

    private RenderTarget2D _activeLayerTarget;
    
    #region Editor

    public UserInterfaceNode WorldMenu;
    public World World;
    public TileSet TileSet;
    public Camera _prevCamera;
    public CommandInvoker CommandInvoker;
    public bool InBounds => GetSelectedCellX() != -1 && GetSelectedCellY() != -1;
    public bool Test = false;

    public int SelectedWorldCellXIndex = 0;
    public int SelectedWorldCellYIndex = 0;

    public int SelectedTilesCellXIndex = 0;
    public int SelectedTilesCellYIndex = 0;

    public uint SelectedTileId = 0;

    public EditorMode EditorMode = EditorMode.World;
    public EditMode EditMode = EditMode.Set;

    public bool ShowMirrorState = false;
    public int ActiveLayer = 0;
    public bool ShowAllLayers = true;

    #endregion

    #region UserInterface

    public Binding<object> EditorModeButtonText;
    public Binding<object> ActiveLayerBinding;
    public Binding<bool> ShowToolButtons = new(true);

    #endregion

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnClientSizeChanged;
    }

    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        Console.WriteLine(GraphicsDevice.Viewport.Bounds);
    }

    protected override void Initialize()
    {
        _activeLayerTarget = new RenderTarget2D(GraphicsDevice, 512, 512);
        
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);

        _camera = new Camera(GraphicsDevice);
        _prevCamera = _camera.Copy();
        _uiRenderer = new UserInterfaceRenderer();
        CommandInvoker = new CommandInvoker();
        
        Context.Pixel = _pixel;
        Context.GraphicsDevice = GraphicsDevice;
        Context.Input = new InputHandle();
        Context.ContentManager = Content;

        EditorModeButtonText = new Binding<object>(EditorMode == EditorMode.Tiles ? "Go To World Mode" : "Go To Tile Mode");
        ActiveLayerBinding = new Binding<object>(ActiveLayer);

        WorldMenu = new HStack(
                new VStack(
                    new Button(
                            new Label()
                            .SetTextBinding(EditorModeButtonText)
                        )
                        .OnClick(() =>
                        {
                            var temp = _camera.Copy();
                            _camera = _prevCamera;
                            _prevCamera = temp;
                            EditorMode = EditorMode == EditorMode.Tiles ? EditorMode.World : EditorMode.Tiles;
                            EditorModeButtonText.Value =
                                EditorMode == EditorMode.Tiles ? "Go To World Mode" : "Go To Tile Mode";
                            ShowToolButtons.Value = EditorMode == EditorMode.World;
                        }),
                    new HStack(
                        new Button(
                                new Label("Set")
                            )
                            .OnClick(() =>
                            {
                                EditMode = EditMode.Set;
                            })
                            .SetVisibilityBinding(ShowToolButtons)
                    ),
                    new Button(
                            new Label("Remove")
                        )
                        .OnClick(() =>
                        {
                            EditMode = EditMode.Remove;
                        })
                        .SetVisibilityBinding(ShowToolButtons),
                    new Button(
                            new Label("Rotate")
                        )
                        .OnClick(() =>
                        {
                            EditMode = EditMode.Rotate;
                        })
                        .SetVisibilityBinding(ShowToolButtons),
                    new Button(
                            new Label("Copy")
                        )
                        .SetVisibilityBinding(ShowToolButtons),
                    new VStack(
                            new Label("Layer Settings")
                                .SetVisibilityBinding(ShowToolButtons),
                            new Button(
                                    new Label("Add")
                                )
                                .SetVisibilityBinding(ShowToolButtons),
                            new Button(
                                    new Label("Remove")
                                )
                                .SetVisibilityBinding(ShowToolButtons),
                            new HStack(
                                new VStack(
                                    new Button(
                                            new Label("Up")
                                        )
                                        .OnClick(() =>
                                        {
                                            ActiveLayer = Math.Clamp(++ActiveLayer, 0, World.LayerCount - 1);
                                            ActiveLayerBinding.Value = ActiveLayer;
                                        })
                                        .SetVisibilityBinding(ShowToolButtons),
                                    new Button(
                                            new Label("Down")
                                        )
                                        .OnClick(() =>
                                        {
                                            ActiveLayer = Math.Clamp(--ActiveLayer, 0, World.LayerCount - 1);
                                            ActiveLayerBinding.Value = ActiveLayer;
                                        })
                                        .SetVisibilityBinding(ShowToolButtons)
                                )
                                .SetSpacing(5),
                                new Label()
                                    .SetTextBinding(ActiveLayerBinding)
                            )
                            .SetAlignment(Alignment.Center),
                            new Button(
                                    new Label("Toogle Render Mode")
                                )
                                .OnClick(() =>
                                {
                                    ShowAllLayers = !ShowAllLayers;
                                })
                                .SetVisibilityBinding(ShowToolButtons)
                        )
                        .SetSpacing(5)
                        .SetPaddingTop(25)
                )
                .SetSpacing(10),
                new Button(
                        new Label("Save The Map")
                    )
                    .OnClick(() => { WorldLoader.WriteToFile(World, "Assets/output.json"); })
                    .SetVisibilityBinding(ShowToolButtons)
            )
            .SetSpacing(10)
            .SetPadding(10);

         

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("default_font");

        _uiRenderer.ButtonTile = EditorUtils.LoadTextureFromPath("Assets/button_nt.png", Context.GraphicsDevice);
        _uiRenderer.Images.Add("microsoft", EditorUtils.LoadTextureFromPath("Assets/Microsoft.png", Context.GraphicsDevice));
        _uiRenderer.Images.Add("lists", EditorUtils.LoadTextureFromPath("Assets/Lists.png", Context.GraphicsDevice));
        _uiRenderer.Images.Add("search", EditorUtils.LoadTextureFromPath("Assets/Search.png", Context.GraphicsDevice));

        Context.Fonts["default"] = _font;
        Context.Fonts["fa-solid"] = Content.Load<SpriteFont>("faSolid");
        Context.Fonts["fa-regular"] = Content.Load<SpriteFont>("faRegular");
        
        World = WorldLoader.ReadFromFile("Assets/output.json");
        TileSet = TileSet.ReadFromFile("Assets/tileSet.json");

        _camera.X = -Context.GraphicsDevice.Viewport.Bounds.Center.X;
        _camera.Y = -Context.GraphicsDevice.Viewport.Bounds.Center.Y;

        _camera.X += World.Width * World.TileSize / 2f;
        _camera.Y += World.Height * World.TileSize / 2f;
        
        _prevCamera.X = -Context.GraphicsDevice.Viewport.Bounds.Center.X;
        _prevCamera.Y = -Context.GraphicsDevice.Viewport.Bounds.Center.Y;

        _prevCamera.X += TileSet.TilesPerRow * TileSet.TileWidth / 2f;
        _prevCamera.Y += TileSet.TilesPerColumn * TileSet.TileHeight / 2f;
        
        
        
    }

    protected override void Update(GameTime gameTime)
    {
        _delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (!IsActive)
        {
            return;
        }

        Context.Input.Update(gameTime, _delta);

        if (Context.Input.IsLeftMouseDown())
        {
            Console.WriteLine(Context.Input.GetMousePosition());
        }
        
        _uiRenderer.Calculate(WorldMenu);

        if (Context.Input.GetMousePosition().Y > 0 && GraphicsDevice.Viewport.Bounds.Contains(Context.Input.GetMousePosition()))
        {
            var result = _uiRenderer.HitTest(WorldMenu);
            Console.WriteLine(result);
            if (result)
            {
                _uiRenderer.HandleInput(WorldMenu);
            }
            else
            {
                HandleInput();
            }
        }

        _camera.Update(gameTime, _delta);

        if (EditMode == EditMode.Rotate && !ShowMirrorState)
        {
            ShowMirrorState = true;
        }
        else if (EditMode != EditMode.Rotate && ShowMirrorState)
        {
            ShowMirrorState = false;
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        DrawActiveLayerTarget(_spriteBatch, gameTime, _delta);

        GraphicsDevice.Clear(Theme.DarkerBrown);

        DrawWithMatrix(_spriteBatch, gameTime, _delta);
        DrawWithoutMatrix(_spriteBatch, gameTime, _delta);
        
        _spriteBatch.Begin(SpriteSortMode.Immediate);
        //_spriteBatch.Draw(_activeLayerTarget, Vector2.Zero, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawActiveLayerTarget(SpriteBatch spriteBatch, GameTime gameTime, float delta)
    {
        GraphicsDevice.SetRenderTarget(_activeLayerTarget);
        GraphicsDevice.Clear(Color.Transparent);
        _spriteBatch.Begin();
        
        if (EditorMode == EditorMode.World)
        {
            for (int y = 0; y < World.Height; y++)
            {
                for (int x = 0; x < World.Width; x++)
                {
                    uint tileId = World.GetTileTexture(x, y, ActiveLayer);
                    if (tileId != 0u)
                    {
                        Rectangle tileSrc =
                            GetSourceRectangle(tileId, World.TileSize, TileSet.TilesPerRow * World.TileSize);
                        
                        spriteBatch.Draw(TileSet.Texture2D, (new Vector2(x, y) * World.TileSize).ToPoint().ToVector2(),
                            tileSrc, Color.White, 0f, Vector2.Zero, Vector2.One,
                            (SpriteEffects)World.GetTileMirror(x, y, ActiveLayer), 0f);
                    }
                }
            }
        }
        
        _spriteBatch.End();
        GraphicsDevice.SetRenderTarget(null);
    }

    private int GetSelectedCellX()
    {
        Vector2 mousePosition = Context.Input.GetMousePosition().ToVector2();
        Vector2 worldPosition = EditorUtils.MapScreenToWorld(mousePosition, _camera);
        int xIndex;
        if (EditorMode == EditorMode.World)
        {
            if (worldPosition.X < 0 || worldPosition.X >= World.Width * World.TileSize)
            {
                return -1;
            }

            xIndex = (int)(worldPosition.X / World.TileSize);
            return xIndex;
        }
        else
        {
            if (worldPosition.X < 0 || worldPosition.X >= TileSet.TilesPerRow * TileSet.TileWidth)
            {
                return -1;
            }

            xIndex = (int)(worldPosition.X / TileSet.TileWidth);
            return xIndex;
        }
    }

    private int GetSelectedCellY()
    {
        Vector2 mousePosition = Context.Input.GetMousePosition().ToVector2();
        Vector2 worldPosition = EditorUtils.MapScreenToWorld(mousePosition, _camera);
        int yIndex;
        if (EditorMode == EditorMode.World)
        {
            if (worldPosition.Y < 0 || worldPosition.Y >= World.Height * World.TileSize)
            {
                return -1;
            }

            yIndex = (int)(worldPosition.Y / World.TileSize);
            return yIndex;
        }
        else
        {
            if (worldPosition.Y < 0 || worldPosition.Y >= TileSet.TilesPerColumn * TileSet.TileHeight)
            {
                return -1;
            }

            yIndex = (int)(worldPosition.Y / TileSet.TileHeight);
            return yIndex;
        }
    }

    private void HandleInput()
    {
        if (Context.Input.IsKeyPressed(Keys.PageUp))
        {
            _camera.ZoomIn();
        }
        else if (Context.Input.IsKeyPressed(Keys.PageDown))
        {
            _camera.ZoomOut();
        }

        if (Context.Input.GetMouseWheelValueDelta() > 0)
        {
            var mousePos = Context.Input.GetMousePosition().ToVector2();
            var oldWorldMousePos = EditorUtils.MapScreenToWorld(mousePos, _camera);
            _camera.ZoomIn();
            var newWorldMousePos = EditorUtils.MapScreenToWorld(mousePos, _camera);
            var diff = oldWorldMousePos - newWorldMousePos;
            _camera.X += diff.X;
            _camera.Y += diff.Y;
        }
        else if (Context.Input.GetMouseWheelValueDelta() < 0)
        {
            var mousePos = Context.Input.GetMousePosition().ToVector2();
            var oldWorldMousePos = EditorUtils.MapScreenToWorld(mousePos, _camera);
            _camera.ZoomOut();
            var newWorldMousePos = EditorUtils.MapScreenToWorld(mousePos, _camera);
            var diff = oldWorldMousePos - newWorldMousePos;
            _camera.X += diff.X;
            _camera.Y += diff.Y;
        }

        if (Context.Input.IsMiddleMouseDown())
        {
            var delta = Context.Input.GetMouseDelta().ToVector2() / _camera.Zoom;
            _camera.X -= delta.X;
            _camera.Y -= delta.Y;
        }

        if (Context.Input.IsKeyDown(Keys.LeftControl))
        {
            if (Context.Input.IsKeyPressed(Keys.Z))
            {
                CommandInvoker.Undo();
            }
        }
        
        if (Context.Input.IsLeftMouseDown())
        {
            if (InBounds)
            {
                if (EditorMode == EditorMode.Tiles)
                {
                    var tileId = (uint)(GetSelectedCellY() * TileSet.TilesPerRow + GetSelectedCellX()) + 1;
                    if (tileId <= TileSet.TileCount)
                    {
                        SelectedTileId = tileId;
                    }
                }
                else if (EditorMode == EditorMode.World && EditMode == EditMode.Set)
                {
                    var oldTile = World.GetTileTexture(GetSelectedCellX(), GetSelectedCellY(), ActiveLayer);
                    if (oldTile != SelectedTileId && SelectedTileId != 0u)
                    {
                        var command = new SetTileCommand(World, GetSelectedCellX(), GetSelectedCellY(), ActiveLayer,
                            oldTile, SelectedTileId);
                        CommandInvoker.ExecuteCommand(command);
                    }
                }
                else if (EditorMode == EditorMode.World)
                {
                    if (EditMode == EditMode.Remove)
                    {
                        World.SetTileTexture(GetSelectedCellX(), GetSelectedCellY(), 0, ActiveLayer);
                    }
                }
            }
        }
        
        if (Context.Input.IsLeftMousePressed())
        {
            if (InBounds)
            {
                if (EditorMode == EditorMode.World)
                {
                    if (EditMode == EditMode.Rotate)
                    {
                        World.Rotate(GetSelectedCellX(), GetSelectedCellY(), ActiveLayer);
                    }
                }
            }
        }

        if (EditorMode == EditorMode.World)
        {
            HandleWorldModeInput();
            if (Context.Input.IsKeyPressed(Keys.S))
            {
                WorldLoader.WriteToFile(World, "Assets/output.json");
            }
        }
        else if (EditorMode == EditorMode.Tiles)
        {
            //HandleTilesModeInput();
        }
    }

    private void HandleWorldModeInput()
    {
        if (Context.Input.IsKeyDown(Keys.LeftControl) || Context.Input.IsKeyDown(Keys.RightControl))
        {
            if (Context.Input.IsKeyPressed(Keys.R))
            {
                if (Context.Input.IsKeyDown(Keys.LeftShift))
                {
                    ShowMirrorState = !ShowMirrorState;
                }
                else
                {
                    var mirrorState = World.GetTileMirror(GetSelectedCellX(), GetSelectedCellY(), ActiveLayer);
                    uint mirrorValue = 0u;
                    switch (mirrorState)
                    {
                        case 0b00:
                            mirrorValue = 0b01;
                            break;
                        case 0b01:
                            mirrorValue = 0b11;
                            break;
                        case 0b10:
                            mirrorValue = 0b00;
                            break;
                        case 0b11:
                            mirrorValue = 0b10;
                            break;
                        default:
                            mirrorValue = 0u;
                            break;
                    }

                    World.SetTileMirror(GetSelectedCellX(), GetSelectedCellY(), mirrorValue, ActiveLayer);
                }
            }

            if (Context.Input.IsKeyPressed(Keys.RightShift))
            {
                ShowAllLayers = !ShowAllLayers;
                return;
            }

            if (Context.Input.IsKeyPressed(Keys.Up))
            {
                ActiveLayer = Math.Clamp(++ActiveLayer, 0, World.LayerCount - 1);
            }

            if (Context.Input.IsKeyPressed(Keys.Down))
            {
                ActiveLayer = Math.Clamp(--ActiveLayer, 0, World.LayerCount - 1);
            }

            return;
        }
    }

    private void HandleTilesModeInput()
    {
        if (Context.Input.IsKeyPressed(Keys.Up))
        {
            SelectedTilesCellYIndex = Math.Clamp(--SelectedTilesCellYIndex, 0, TileSet.TilesPerColumn - 1);
        }

        if (Context.Input.IsKeyPressed(Keys.Down))
        {
            SelectedTilesCellYIndex = Math.Clamp(++SelectedTilesCellYIndex, 0, TileSet.TilesPerColumn - 1);
        }

        if (Context.Input.IsKeyPressed(Keys.Left))
        {
            SelectedTilesCellXIndex = Math.Clamp(--SelectedTilesCellXIndex, 0, TileSet.TilesPerRow - 1);
        }

        if (Context.Input.IsKeyPressed(Keys.Right))
        {
            SelectedTilesCellXIndex = Math.Clamp(++SelectedTilesCellXIndex, 0, TileSet.TilesPerRow - 1);
        }

        if (Context.Input.IsKeyPressed(Keys.M))
        {
            EditorMode = EditorMode.World;
        }

        if (Context.Input.IsKeyPressed(Keys.Enter))
        {
            var tileId = (uint)(SelectedTilesCellYIndex * TileSet.TilesPerRow + SelectedTilesCellXIndex) + 1;
            if (tileId <= TileSet.TileCount)
            {
                SelectedTileId = tileId;
            }
        }
    }

    protected void DrawSelection(int cellXIndex, int cellYIndex, Color tint)
    {
        Point CellSize = new Point(16);
        int x = cellXIndex * CellSize.X;
        int y = cellYIndex * CellSize.Y;
        _spriteBatch.Draw(Context.Pixel, new Rectangle(x, y, CellSize.X, 1), tint);
        _spriteBatch.Draw(Context.Pixel, new Rectangle(x, y + CellSize.Y - 1, CellSize.X, 1), tint);
        _spriteBatch.Draw(Context.Pixel, new Rectangle(x, y, 1, CellSize.Y), tint);
        _spriteBatch.Draw(Context.Pixel, new Rectangle(x + CellSize.X - 1, y, 1, CellSize.Y), tint);
    }

    protected void DrawSelectionR(int cellXIndex, int cellYIndex, Color tint)
    {
        Point CellSize = new Point(16);
        int x = cellXIndex * CellSize.X;
        int y = cellYIndex * CellSize.Y;

        int minWidth = 1;
        int minHeight = 1;
        int maxWidth = CellSize.X;
        int maxHeight = CellSize.Y;

        Point position = EditorUtils.MapWorldToScreen(new Vector2(x, y), _camera).ToPoint();
        x = position.X;
        y = position.Y;

        _spriteBatch.Draw(Context.Pixel, new Rectangle(x, y, CellSize.X * (int)_camera.Zoom, 1), tint);
        _spriteBatch.Draw(Context.Pixel,
            new Rectangle(x, y + CellSize.Y * (int)_camera.Zoom - 1, CellSize.X * (int)_camera.Zoom, 1), tint);
        _spriteBatch.Draw(Context.Pixel, new Rectangle(x, y, 1, CellSize.Y * (int)_camera.Zoom), tint);
        _spriteBatch.Draw(Context.Pixel,
            new Rectangle(x + CellSize.X * (int)_camera.Zoom - 1, y, 1, CellSize.Y * (int)_camera.Zoom), tint);
    }

    protected void DrawWithMatrix(SpriteBatch spriteBatch, GameTime gameTime, float delta)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.TransformationMatrix);

        if (EditorMode == EditorMode.World)
        {
            DrawWorldMode(spriteBatch, gameTime, delta);
            spriteBatch.Draw(_activeLayerTarget, Vector2.Zero, Color.White);
        }

        if (EditorMode == EditorMode.Tiles)
        {
            DrawTileMode(spriteBatch, gameTime, delta);
        }

        spriteBatch.End();
    }

    protected void DrawWithoutMatrix(SpriteBatch spriteBatch, GameTime gameTime, float delta)
    {
        spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp);
        DrawUserInterface(spriteBatch, gameTime, delta);
        _uiRenderer.Draw(WorldMenu, _spriteBatch, gameTime, _delta);
        spriteBatch.End();
    }

    protected void DrawUserInterface(SpriteBatch spriteBatch, GameTime gameTime, float delta)
    {
        if (EditorMode == EditorMode.World)
        {
            for (int y = 0; y < World.Height; y++)
            {
                for (int x = 0; x < World.Width; x++)
                {
                    uint tileId = World.GetTileTexture(x, y, ActiveLayer);
                    if (tileId != 0u)
                    {
                        if (ShowMirrorState)
                        {
                            _spriteBatch.DrawString(_font,
                                Convert.ToString(World.GetTileMirror(x, y, ActiveLayer), 2).PadLeft(2, '0'),
                                EditorUtils.MapWorldToScreen((new Vector2(x, y) * World.TileSize), _camera).ToPoint()
                                    .ToVector2(), Color.White);
                        }
                    }
                }
            }
        }

        //var position = EditorUtils.MapWorldToScreen((new Vector2(GetSelectedCellX(), GetSelectedCellY())), _camera).ToPoint();
        //DrawSelectionR(position.X, position.Y, Color.Black);
    }

    public void DrawMultipleStrings(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, Color color,
        params string[] strings)
    {
        for (int i = 0; i < strings.Length; i++)
        {
            spriteBatch.DrawString(font, strings[i], position + new Vector2(0, font.LineSpacing * i), color);
        }
    }

    public void DrawMultipleStrings(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, Color color,
        out Rectangle blockBounds, params string[] strings)
    {
        blockBounds = new Rectangle(position.ToPoint(), Point.Zero); // Leeres Rechteck initialisieren

        foreach (string str in strings)
        {
            Vector2 stringSize = font.MeasureString(str);
            Rectangle stringBounds =
                new Rectangle((int)position.X, (int)position.Y, (int)stringSize.X, (int)stringSize.Y);

            // Aktualisieren der gesamten Blockgrenzen, um den aktuellen Text einzuschließen
            blockBounds = Rectangle.Union(blockBounds, stringBounds);

            // Zeichnen des Textes
            spriteBatch.DrawString(font, str, position, color);

            // Aktualisieren der Position für den nächsten Text
            position.Y += stringSize.Y;
        }
    }

    protected void DrawTileMode(SpriteBatch spriteBatch, GameTime gameTime, float delta)
    {
        float amplitude = 0.2f; // Amplitude für die Sinuswelle
        float offset = 0.3f; // Verschiebung für die Sinuswelle

        // Berechnung des Alpha-Werts basierend auf einer Sinusfunktion
        float alpha = offset + amplitude * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds);

        // Begrenzung des Alpha-Werts auf den Bereich von 0.1f bis 0.5f
        alpha = MathHelper.Clamp(alpha, 0.1f, 0.5f);

        spriteBatch.Draw(_pixel,
            new Rectangle(0, 0, TileSet.TilesPerRow * TileSet.TileWidth, TileSet.TilesPerRow * TileSet.TileHeight),
            Color.Black * alpha);
        spriteBatch.Draw(TileSet.Texture2D, Vector2.Zero, Color.White);

        if (InBounds)
        {
            DrawSelection(GetSelectedCellX(), GetSelectedCellY(), Color.White * 0.5f);
        }
    }

    protected void DrawWorldMode(SpriteBatch spriteBatch, GameTime gameTime, float delta)
    {
        var alpha = 0.5f;
        for (int layer = 0; layer < World.LayerCount; layer++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                for (int x = 0; x < World.Width; x++)
                {
                    uint tileId = World.GetTileTexture(x, y, layer);
                    if (tileId != 0u)
                    {
                        Rectangle tileSrc =
                            GetSourceRectangle(tileId, World.TileSize, TileSet.TilesPerRow * World.TileSize);

                        if (layer == ActiveLayer)
                        {
                            alpha = 1.0f;
                        }
                        else
                        {
                            alpha = 0.2f;
                        }

                        if (ShowAllLayers)
                        {
                            alpha = 1.0f;
                        }

                        spriteBatch.Draw(TileSet.Texture2D, (new Vector2(x, y) * World.TileSize).ToPoint().ToVector2(),
                            tileSrc, Color.White * alpha, 0f, Vector2.Zero, Vector2.One,
                            (SpriteEffects)World.GetTileMirror(x, y, layer), 0f);
                    }
                }
            }

            if (ActiveLayer == layer)
            {
                if (InBounds)
                {
                    if (SelectedTileId != 0u)
                    {
                        if (EditMode == EditMode.Set)
                        {
                            Rectangle tileSrc =
                                GetSourceRectangle(SelectedTileId, World.TileSize, TileSet.TilesPerRow * World.TileSize);
                            spriteBatch.Draw(TileSet.Texture2D,
                                (new Vector2(GetSelectedCellX(), GetSelectedCellY()) * World.TileSize).ToPoint().ToVector2(),
                                tileSrc, Color.White * (float)Math.Abs(Math.Sin(gameTime.TotalGameTime.TotalSeconds)));
                        }
                    }

                    DrawSelection(GetSelectedCellX(), GetSelectedCellY(), Color.White * 1f);
                }
            }
        }

        if (InBounds)
        {
            if (SelectedTileId != 0u)
            {
                if (EditMode == EditMode.Set)
                {
                    Rectangle tileSrc =
                        GetSourceRectangle(SelectedTileId, World.TileSize, TileSet.TilesPerRow * World.TileSize);
                    spriteBatch.Draw(TileSet.Texture2D,
                        (new Vector2(GetSelectedCellX(), GetSelectedCellY()) * World.TileSize).ToPoint().ToVector2(),
                        tileSrc, Color.White * (float)Math.Abs(Math.Sin(gameTime.TotalGameTime.TotalSeconds)));
                }
            }

            //DrawSelection(GetSelectedCellX(), GetSelectedCellY(), Color.White * 0.5f);
        }
    }

    protected Rectangle GetSourceRectangle(uint tileId, int tileSize, int atlasWidth)
    {
        tileId--;
        if (tileId < 0)
        {
            throw new Exception();
        }

        int tilesPerRow = atlasWidth / tileSize;
        int tileX = ((int)tileId % tilesPerRow) * tileSize;
        int tileY = ((int)tileId / tilesPerRow) * tileSize;
        return new Rectangle(tileX, tileY, tileSize, tileSize);
    }
}