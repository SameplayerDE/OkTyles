using System;
using System.IO;
using OkTyles.Core;
using Bembelbuben.Core;
using Bembelbuben.Core.Input;
using Bembelbuben.Core.Rendering;
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

    #region Editor

    public UserInterfaceNode NewWorldMenu;
    public UserInterfaceNode ActiveNode;
    public UserInterfaceNode WorldMenu;
    public UserInterfaceNode StartUpMenu;

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

    public EditorMode EditorMode = EditorMode.StartUp;
    public EditMode EditMode = EditMode.Set;
    public World.CollisionMask CollisionMaskBrush = World.CollisionMask.Rectangle;

    public bool ShowMirrorState = false;
    public int ActiveLayer = 0;
    public bool ShowAllLayers = true;

    #endregion

    #region UserInterface

    public Binding<object> EditorModeButtonText;
    public Binding<object> ActiveLayerBinding;
    public Binding<object> LayerCountBinding;
    public Binding<bool> ShowToolButtons = new(true);
    public Binding<bool> ShowCollisionRules = new(false);

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
        PrimitiveRenderer.Initialise(GraphicsDevice);

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

        EditorModeButtonText =
            new Binding<object>(EditorMode == EditorMode.Tiles ? "Go To World Mode" : "Go To Tile Mode");
        ActiveLayerBinding = new Binding<object>(ActiveLayer);
        LayerCountBinding = new Binding<object>(0);

        NewWorldMenu = new VStack(
                new Label("New World"),
                new VStack(
                    new Label("Width of Tile")
                ),
                new VStack(
                    new Label("Height of Tile")
                ),
                new VStack(
                    new Label("Width of Map in Tiles")
                ),
                new VStack(
                    new Label("Height of Map in Tiles")
                )
            )
            .SetPadding(10)
            .SetSpacing(10);

        StartUpMenu = new HStack(
                new Button(new Label("Create Map"))
                    .OnClick(() => { ActiveNode = NewWorldMenu; }),
                new Button(new Label("Load Map"))
                    .OnClick(() =>
                    {
                        ActiveNode = WorldMenu;
                        World = WorldLoader.ReadFromFile("Assets/output.json");
                        TileSet = TileSet.ReadFromFile("Assets/tileSet.json");
                        EditorMode = EditorMode.World;
                    })
            )
            .SetPadding(10)
            .SetSpacing(10);

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
                                .OnClick(() => { EditMode = EditMode.Set; })
                                .SetVisibilityBinding(ShowToolButtons)
                        ),
                        new Button(
                                new Label("Remove")
                            )
                            .OnClick(() => { EditMode = EditMode.Remove; })
                            .SetVisibilityBinding(ShowToolButtons),
                        new Button(
                                new Label("Rotate")
                            )
                            .OnClick(() => { EditMode = EditMode.Rotate; })
                            .SetVisibilityBinding(ShowToolButtons),
                        new Button(
                                new Label("Copy")
                            )
                            .SetVisibilityBinding(ShowToolButtons),
                        new Button(
                                new Label("Collisions")
                            )
                            .OnClick(() =>
                            {
                                EditMode = EditMode.Collision;
                                CollisionMaskBrush = World.CollisionMask.None;
                            })
                            .SetVisibilityBinding(ShowToolButtons),
                        new VStack(
                                new VStack(
                                        new Label("Shapes"),
                                        new Button(
                                                new Label("None")
                                            )
                                            .OnClick(() => { CollisionMaskBrush = World.CollisionMask.None; }),
                                        new Button(
                                                new Label("Full")
                                            )
                                            .OnClick(() => { CollisionMaskBrush = World.CollisionMask.Rectangle; }),
                                        new Button(
                                                new Label("Circle")
                                            )
                                            .OnClick(() => { CollisionMaskBrush = World.CollisionMask.Circle; }),
                                        new Button(
                                                new Label("Slope Top Left")
                                            )
                                            .OnClick(() => { CollisionMaskBrush = World.CollisionMask.SlopeTL; }),
                                        new Button(
                                                new Label("Slope Top Right")
                                            )
                                            .OnClick(() => { CollisionMaskBrush = World.CollisionMask.SlopeTR; }),
                                        new Button(
                                                new Label("Slope Bottom Left")
                                            )
                                            .OnClick(() => { CollisionMaskBrush = World.CollisionMask.SlopeBL; }),
                                        new Button(
                                                new Label("Slope Bottom Right")
                                            )
                                            .OnClick(() => { CollisionMaskBrush = World.CollisionMask.SlopeBR; })
                                    )
                                    .SetSpacing(5)
                                    .SetVisibilityBinding(ShowCollisionRules)
                            )
                            .SetVisibilityBinding(ShowToolButtons),
                        new VStack(
                                new Label("Layer Settings")
                                    .SetVisibilityBinding(ShowToolButtons),
                                new HStack(
                                        new VStack(
                                                new Button(
                                                        new Label("Add")
                                                    )
                                                    .OnClick(() => { World.AddLayer(++ActiveLayer); })
                                                    .SetVisibilityBinding(ShowToolButtons),
                                                new Button(
                                                        new Label("Remove")
                                                    )
                                                    .OnClick(() =>
                                                    {
                                                        if (World.LayerCount > 1)
                                                        {
                                                            World.RemoveLayer(ActiveLayer);
                                                            ActiveLayer = Math.Max(ActiveLayer - 1, 0);
                                                        }
                                                    })
                                                    .SetVisibilityBinding(ShowToolButtons)
                                            )
                                            .SetSpacing(5),
                                        new Label()
                                            .SetTextBinding(LayerCountBinding)
                                            .SetVisibilityBinding(ShowToolButtons)
                                    )
                                    .SetAlignment(Alignment.Center),
                                new HStack(
                                        new VStack(
                                                new Button(
                                                        new Label("Up")
                                                    )
                                                    .OnClick(() =>
                                                    {
                                                        ActiveLayer = Math.Clamp(++ActiveLayer, 0,
                                                            World.LayerCount - 1);
                                                    })
                                                    .SetVisibilityBinding(ShowToolButtons),
                                                new Button(
                                                        new Label("Down")
                                                    )
                                                    .OnClick(() =>
                                                    {
                                                        ActiveLayer = Math.Clamp(--ActiveLayer, 0,
                                                            World.LayerCount - 1);
                                                    })
                                                    .SetVisibilityBinding(ShowToolButtons)
                                            )
                                            .SetSpacing(5),
                                        new Label()
                                            .SetTextBinding(ActiveLayerBinding)
                                            .SetVisibilityBinding(ShowToolButtons)
                                    )
                                    .SetAlignment(Alignment.Center),
                                new HStack(
                                        new VStack(
                                                new Button(
                                                        new Label("Move Up")
                                                    )
                                                    .OnClick(() =>
                                                    {
                                                        var maxLayers = World.LayerCount;
                                                        var currentLayer = ActiveLayer;
                                                        var switchLayer = ActiveLayer + 1;

                                                        if (switchLayer < maxLayers)
                                                        {
                                                            (World.Layers[switchLayer],
                                                                World.Layers[currentLayer]) = (
                                                                World.Layers[currentLayer],
                                                                World.Layers[switchLayer]);
                                                            ActiveLayer = switchLayer;
                                                        }
                                                    })
                                                    .SetVisibilityBinding(ShowToolButtons),
                                                new Button(
                                                        new Label("Move Down")
                                                    )
                                                    .OnClick(() =>
                                                    {
                                                        var maxLayers = World.LayerCount;
                                                        var currentLayer = ActiveLayer;
                                                        var switchLayer = ActiveLayer - 1;

                                                        if (switchLayer >= 0)
                                                        {
                                                            (World.Layers[switchLayer],
                                                                World.Layers[currentLayer]) = (
                                                                World.Layers[currentLayer],
                                                                World.Layers[switchLayer]);
                                                            ActiveLayer = switchLayer;
                                                        }
                                                    })
                                                    .SetVisibilityBinding(ShowToolButtons)
                                            )
                                            .SetSpacing(5)
                                    )
                                    .SetAlignment(Alignment.Center),
                                new Button(
                                        new Label("Toogle Render Mode")
                                    )
                                    .OnClick(() => { ShowAllLayers = !ShowAllLayers; })
                                    .SetVisibilityBinding(ShowToolButtons)
                            )
                            .SetSpacing(15)
                            .SetPaddingTop(25)
                    )
                    .SetSpacing(10),
                new Button(
                        new Label("Save The Map")
                    )
                    .OnClick(() => { WorldLoader.WriteToFile(World, "Assets/output.json"); })
                    .SetVisibilityBinding(ShowToolButtons),
                new Button(
                        new Label("Export Each Laye As Image")
                    )
                    .OnClick(() => { SaveLayersAsTextures(); })
                    .SetVisibilityBinding(ShowToolButtons)
            )
            .SetSpacing(10)
            .SetPadding(10);


        ActiveNode = WorldMenu;
        World = WorldLoader.ReadFromFile("Assets/output.json");
        TileSet = TileSet.ReadFromFile("Assets/tileSet.json");
        EditorMode = EditorMode.World;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("default_font");

        _uiRenderer.ButtonTile = EditorUtils.LoadTextureFromPath("Assets/button_nt.png", Context.GraphicsDevice);
        _uiRenderer.AddImage("microsoft",
            EditorUtils.LoadTextureFromPath("Assets/Microsoft.png", Context.GraphicsDevice));
        _uiRenderer.AddImage("lists", EditorUtils.LoadTextureFromPath("Assets/Lists.png", Context.GraphicsDevice));
        _uiRenderer.AddImage("search", EditorUtils.LoadTextureFromPath("Assets/Search.png", Context.GraphicsDevice));

        Context.Fonts["default"] = _font;
        Context.Fonts["fa-solid"] = Content.Load<SpriteFont>("faSolid");
        Context.Fonts["fa-regular"] = Content.Load<SpriteFont>("faRegular");

        LayerCountBinding.Value = World.LayerCount;

        _camera.X += World.Width * World.TileSize / 2f;
        _camera.Y += World.Height * World.TileSize / 2f;

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

        _uiRenderer.CalculateLayout(ActiveNode);
        if (Context.Input.GetMousePosition().Y > 0 &&
            GraphicsDevice.Viewport.Bounds.Contains(Context.Input.GetMousePosition()))
        {
            var result = _uiRenderer.HitTest(ActiveNode);
            Console.WriteLine(result);
            if (result)
            {
                _uiRenderer.HandleInput(ActiveNode);
            }
            else
            {
                HandleInput();
            }
        }

        _uiRenderer.CalculateLayout(ActiveNode);

        if (EditMode == EditMode.Rotate && !ShowMirrorState)
        {
            ShowMirrorState = true;
        }
        else if (EditMode != EditMode.Rotate && ShowMirrorState)
        {
            ShowMirrorState = false;
        }

        if (EditMode == EditMode.Collision && !ShowCollisionRules.Value)
        {
            ShowCollisionRules.Value = true;
        }
        else if (EditMode != EditMode.Collision && ShowCollisionRules.Value)
        {
            ShowCollisionRules.Value = false;
        }

        ActiveLayerBinding.Value = ActiveLayer + 1;
        LayerCountBinding.Value = World.LayerCount;

        _camera.Update(gameTime, _delta);

        Console.WriteLine(_camera.Zoom);
        Console.WriteLine(_camera.X);
        Console.WriteLine(_camera.Y);
        Console.WriteLine("-----");

        PrimitiveRenderer.Scale = 1 / _camera.Zoom;
        PrimitiveRenderer.ViewOffset = new Vector3(_camera.X, _camera.Y, 0);
        PrimitiveRenderer.UpdateDefaultCamera();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Theme.DarkerBrown);

        DrawWithMatrix(_spriteBatch, gameTime, _delta);

        if (EditMode == EditMode.Collision && EditorMode == EditorMode.World)
        {
            if (InBounds)
            {
                DrawCollisionSelection(gameTime, _delta);
            }

            DrawCollisionLayer(gameTime, _delta);
        }

        DrawWithoutMatrix(_spriteBatch, gameTime, _delta);


        base.Draw(gameTime);
    }

    private void DrawCollisionSelection(GameTime gameTime, float delta)
    {
        
                int x = GetSelectedCellX();
                int y = GetSelectedCellY();
                Color color = Color.Black * 0.25f;

                //Draw selection
                if (CollisionMaskBrush == World.CollisionMask.Rectangle)
                {
                    PrimitiveRenderer.DrawRectF(
                        null,
                        color,
                        new Rectangle(GetSelectedCellX() * World.TileSize, GetSelectedCellY() * World.TileSize,
                            World.TileSize, World.TileSize)
                    );
                }

                if (CollisionMaskBrush == World.CollisionMask.Circle)
                {
                    PrimitiveRenderer.DrawCircleF(
                        null,
                        color,
                        new Vector2(x * World.TileSize, y * World.TileSize) + new Vector2(World.TileSize) / 2,
                        World.TileSize / 2,
                        2
                    );
                }

                if (CollisionMaskBrush == World.CollisionMask.SlopeTL)
                {
                    PrimitiveRenderer.DrawTriangleF(
                        null,
                        color,
                        new Vector2(x * World.TileSize, y * World.TileSize),
                        new Vector2(x * World.TileSize + World.TileSize, y * World.TileSize),
                        new Vector2(x * World.TileSize, y * World.TileSize + World.TileSize)
                    );
                }

                if (CollisionMaskBrush == World.CollisionMask.SlopeTR)
                {
                    PrimitiveRenderer.DrawTriangleF(
                        null,
                        color,
                        new Vector2(x * World.TileSize, y * World.TileSize),
                        new Vector2(x * World.TileSize + World.TileSize, y * World.TileSize),
                        new Vector2(x * World.TileSize + World.TileSize, y * World.TileSize + World.TileSize)
                    );
                }

                if (CollisionMaskBrush == World.CollisionMask.SlopeBL)
                {
                    PrimitiveRenderer.DrawTriangleF(
                        null,
                        color,
                        new Vector2(x * World.TileSize, y * World.TileSize),
                        new Vector2(x * World.TileSize + World.TileSize, y * World.TileSize + World.TileSize),
                        new Vector2(x * World.TileSize, y * World.TileSize + World.TileSize)
                    );
                }

                if (CollisionMaskBrush == World.CollisionMask.SlopeBR)
                {
                    PrimitiveRenderer.DrawTriangleF(
                        null,
                        color,
                        new Vector2(x * World.TileSize + World.TileSize, y * World.TileSize),
                        new Vector2(x * World.TileSize + World.TileSize, y * World.TileSize + World.TileSize),
                        new Vector2(x * World.TileSize, y * World.TileSize + World.TileSize)
                    );
                }
    }

    private void DrawCollisionLayer(GameTime gameTime, float delta)
    {
        for (int y = 0; y < World.Height; y++)
        {
            for (int x = 0; x < World.Width; x++)
            {
                World.CollisionMask collisionMask = World.GetTileCollision(x, y, ActiveLayer);
                if (collisionMask != 0u)
                {
                    if (collisionMask == World.CollisionMask.Rectangle)
                    {
                        PrimitiveRenderer.DrawRectF(
                            null,
                            Color.Red * 0.5f,
                            new Rectangle(x * World.TileSize, y * World.TileSize, World.TileSize, World.TileSize)
                        );
                    }

                    if (collisionMask == World.CollisionMask.Circle)
                    {
                        PrimitiveRenderer.DrawCircleF(
                            null,
                            Color.Red * 0.5f,
                            new Vector2(x * World.TileSize, y * World.TileSize) + new Vector2(World.TileSize) / 2,
                            World.TileSize / 2,
                            2
                        );
                    }

                    if (collisionMask == World.CollisionMask.SlopeTL)
                    {
                        PrimitiveRenderer.DrawTriangleF(
                            null,
                            Color.Red * 0.5f,
                            new Vector2(x * World.TileSize, y * World.TileSize),
                            new Vector2(x * World.TileSize + World.TileSize, y * World.TileSize),
                            new Vector2(x * World.TileSize, y * World.TileSize + World.TileSize)
                        );
                    }

                    if (collisionMask == World.CollisionMask.SlopeTR)
                    {
                        PrimitiveRenderer.DrawTriangleF(
                            null,
                            Color.Red * 0.5f,
                            new Vector2(x * World.TileSize, y * World.TileSize),
                            new Vector2(x * World.TileSize + World.TileSize, y * World.TileSize),
                            new Vector2(x * World.TileSize + World.TileSize, y * World.TileSize + World.TileSize)
                        );
                    }

                    if (collisionMask == World.CollisionMask.SlopeBL)
                    {
                        PrimitiveRenderer.DrawTriangleF(
                            null,
                            Color.Red * 0.5f,
                            new Vector2(x * World.TileSize, y * World.TileSize),
                            new Vector2(x * World.TileSize + World.TileSize, y * World.TileSize + World.TileSize),
                            new Vector2(x * World.TileSize, y * World.TileSize + World.TileSize)
                        );
                    }

                    if (collisionMask == World.CollisionMask.SlopeBR)
                    {
                        PrimitiveRenderer.DrawTriangleF(
                            null,
                            Color.Red * 0.5f,
                            new Vector2(x * World.TileSize + World.TileSize, y * World.TileSize),
                            new Vector2(x * World.TileSize + World.TileSize, y * World.TileSize + World.TileSize),
                            new Vector2(x * World.TileSize, y * World.TileSize + World.TileSize)
                        );
                    }
                }
            }
        }
    }

    private void SaveLayersAsTextures()
    {
        // Erstelle ein Verzeichnis zum Speichern der Texturen, falls es nicht existiert
        string directoryPath = "LayerTextures";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Iteriere über alle Layer und speichere sie als separate Texturen
        for (int layerIndex = 0; layerIndex < World.LayerCount; layerIndex++)
        {
            // Überprüfe, ob der Layer Daten enthält
            bool hasData = CheckLayerForData(layerIndex);
            if (!hasData)
            {
                continue; // Springe zum nächsten Layer, wenn dieser keine Daten enthält
            }

            // Erstelle eine Render-Target-Texture für den aktuellen Layer
            RenderTarget2D layerTexture = new RenderTarget2D(GraphicsDevice, World.Width * World.TileSize,
                World.Height * World.TileSize);

            // Setze das Renderziel auf die aktuelle Layer-Texture
            GraphicsDevice.SetRenderTarget(layerTexture);
            GraphicsDevice.Clear(Color.Transparent); // Setze die Hintergrundfarbe auf Transparent

            // Beginne das Zeichnen auf die aktuelle Layer-Texture
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            // Zeichne den aktuellen Layer auf die Layer-Texture
            DrawWorldLayer(layerIndex);

            // Beende das Zeichnen auf die Layer-Texture
            _spriteBatch.End();

            // Setze das Renderziel zurück auf den Standardbildschirm
            GraphicsDevice.SetRenderTarget(null);

            // Speichere die Layer-Texture in einer Datei
            string filePath = Path.Combine(directoryPath, $"Layer_{layerIndex}.png");
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                layerTexture.SaveAsPng(stream, layerTexture.Width, layerTexture.Height);
            }
        }
    }

    private bool CheckLayerForData(int layerIndex)
    {
        for (int y = 0; y < World.Height; y++)
        {
            for (int x = 0; x < World.Width; x++)
            {
                uint tileId = World.GetTileTexture(x, y, layerIndex);
                if (tileId != 0u)
                {
                    return true; // Der Layer enthält mindestens ein Kachel-Datum
                }
            }
        }

        return false; // Der Layer enthält keine Kachel-Daten
    }

    private void DrawWorldLayer(int layerIndex)
    {
        for (int y = 0; y < World.Height; y++)
        {
            for (int x = 0; x < World.Width; x++)
            {
                uint tileId = World.GetTileTexture(x, y, layerIndex);
                if (tileId != 0u)
                {
                    Rectangle tileSrc =
                        GetSourceRectangle(tileId, World.TileSize, TileSet.TilesPerRow * World.TileSize);
                    _spriteBatch.Draw(TileSet.Texture2D, (new Vector2(x, y) * World.TileSize).ToPoint().ToVector2(),
                        tileSrc, Color.White, 0f, Vector2.Zero, Vector2.One,
                        (SpriteEffects)World.GetTileMirror(x, y, layerIndex), 0f);
                }
            }
        }
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
            _camera.X = (float)Math.Round(_camera.X);
            _camera.Y = (float)Math.Round(_camera.Y);
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

            _camera.X = (float)Math.Round(_camera.X);
            _camera.Y = (float)Math.Round(_camera.Y);
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
                        var command = new ChangeTileDataCommand(World, GetSelectedCellX(), GetSelectedCellY(),
                            ActiveLayer,
                            oldTile, SelectedTileId);
                        CommandInvoker.ExecuteCommand(command);
                    }
                }
                else if (EditorMode == EditorMode.World)
                {
                    if (EditMode == EditMode.Remove)
                    {
                        var oldTile = World.GetTileData(GetSelectedCellX(), GetSelectedCellY(), ActiveLayer);
                        if (oldTile != 0u)
                        {
                            var command = new ChangeTileDataCommand(World, GetSelectedCellX(), GetSelectedCellY(),
                                ActiveLayer,
                                oldTile, 0u);
                            CommandInvoker.ExecuteCommand(command);
                        }
                    }

                    if (EditMode == EditMode.Collision)
                    {
                        World.SetTileCollision(GetSelectedCellX(), GetSelectedCellY(), CollisionMaskBrush, ActiveLayer);
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
                        var oldTile = World.GetTileData(GetSelectedCellX(), GetSelectedCellY(), ActiveLayer);
                        World.Rotate(GetSelectedCellX(), GetSelectedCellY(), ActiveLayer);
                        var newTile = World.GetTileData(GetSelectedCellX(), GetSelectedCellY(), ActiveLayer);
                        if (oldTile != newTile)
                        {
                            var command = new ChangeTileDataCommand(World, GetSelectedCellX(), GetSelectedCellY(),
                                ActiveLayer,
                                oldTile, newTile);
                            CommandInvoker.ExecuteCommand(command);
                        }
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

            if (Context.Input.IsKeyPressed(Keys.F1))
            {
                SaveLayersAsTextures();
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
        if (World == null && TileSet == null)
        {
            return;
        }

        spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: _camera.TransformationMatrix);

        if (EditorMode == EditorMode.World)
        {
            DrawWorldMode(spriteBatch, gameTime, delta);
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
        _uiRenderer.Draw(ActiveNode, _spriteBatch, gameTime, _delta);
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

                        if (ShowAllLayers)
                        {
                            alpha = 1.0f;
                        }
                        else
                        {
                            // Calculate alpha based on layer distance from active layer
                            float distanceFactor = Math.Abs(layer - ActiveLayer) / (float)World.LayerCount;
                            alpha = 1.0f - distanceFactor * 0.8f;
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
                                GetSourceRectangle(SelectedTileId, World.TileSize,
                                    TileSet.TilesPerRow * World.TileSize);
                            spriteBatch.Draw(TileSet.Texture2D,
                                (new Vector2(GetSelectedCellX(), GetSelectedCellY()) * World.TileSize).ToPoint()
                                .ToVector2(),
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