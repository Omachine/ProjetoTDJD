# Monogame Trabalho de Tecnicas e Desenvolvimento de Jogos Digitais

**Gonçalo Cruz Moreira Nº25965**

//Descriçao da implementação do jogo
//decisões tomadas e instruções do jogo
//Analise dos ficheiros
//Organização das pastas do jogo
//Analise dos códigos Disponibilizados

# Sol Standard

O jogo escolhido foi Sol Standard, disponivel no itch.io. Este é um jogo competitivo de táticas. O jogo apresenta várias personagens 
com habilidades únicas para conseguir vencer um inimigo. Também contém vários mapas, inimigos controlados por AI e objetos 
com o qual consegue interagir. Neste jogo, tanto o teclado como o gamepad são suportados, sendo que é permitido alterar a sua interface, dependendo 
do que estiver a ser usado. Porém, é aconselhado usar um comando de Xbox. Teclado é capaz de controlar 2 jogadores sendo possivel lutar contra si próprio.

A avaliação deste projeto será feita tendo em conta as componentes base do jogo tanto como os controlos e a interface. 

O repositório do jogo apresenta tanto o jogo em si como uma versão teste.

## Pastas e Organização

Os ficheiros do jogo estão organizados em pastas que dividem de forma atenta todos os componentes do jogo. O desenvolvedor separou em Containers, Entity, HUD, Map e Utility. A pasta Containers tem o menu inicial, a seleção de níveis, a mudança de botões de controlos e cenário. A pasta Entity contém tudo o que possa ser considerado uma entidade como decorações do mapa, unidades, e classes de interação com essas entidades. Na pasta HUD contém o que pode ser disposto no ecrã com o menu de ações, menu inicial e o menu de pausa. Na pasta Map tem o controlo da câmara do mapa, o mapa em si e todos os seus conteúdos. Já na pasta Utility está contida grande parte das funções que são usadas em todos os componentes do jogo como os do menu, ações dos jogadores e dos inimigos, as funções de carregar os próprios sprites e da conexão a rede para que se possa jogar em multiplayer.

### Inputs

Nessa versão do jogo encontra-se os controlos do jogo dentro das classes `ControlMapper`, `GameControlParser`, `InputKey`, `KeyboardController` no caso da utilização de teclado e as classes `GamepadController` e `InputButton` no caso de comando de Xbox.

`ControlMapper.cs`

Código para o tipo de click que possa ser usado e quais inputs existem com o Dingle press presente como exemplo.

```cs
public enum PressType
{
DelayedRepeat,
InstantRepeat,
Single
}

public enum Input
{
None,

CursorUp,
CursorDown,
CursorLeft,
CursorRight,

CameraUp,
CameraDown,
CameraLeft,
CameraRight,

Confirm,
Cancel,

PreviewUnit,
PreviewItem,

Status,
Menu,

TabLeft,
TabRight,
ZoomOut,
ZoomIn
}


protected bool SinglePress(GameControl control, bool incrementInputCounter)
{
//Press just once on input down; do not repeat
if (control.Pressed)
{
if (control.InputCounter == 0)
{
if (incrementInputCounter) control.IncrementInputCounter();
InputIconProvider.UpdateLastInputType(ControlType);
return true;
}
}
```

`GameControlParser.cs`

```cs
namespace SolStandard.Utility.Inputs
{
public class GameControlParser : ControlMapper
{
private readonly Dictionary<Input, GameControl> buttonMap;

public GameControlParser(IController controller) : base(controller)
{
buttonMap = new Dictionary<Input, GameControl>
{
{Input.CursorUp, controller.CursorUp},
{Input.CursorDown, controller.CursorDown},
{Input.CursorLeft, controller.CursorLeft},
{Input.CursorRight, controller.CursorRight},

{Input.CameraUp, controller.CameraUp},
{Input.CameraDown, controller.CameraDown},
{Input.CameraLeft, controller.CameraLeft},
{Input.CameraRight, controller.CameraRight},

{Input.Menu, controller.Menu},
{Input.Status, controller.Status},

{Input.Confirm, controller.Confirm},
{Input.Cancel, controller.Cancel},
{Input.PreviewUnit, controller.PreviewUnit},
{Input.PreviewItem, controller.PreviewItem},

{Input.TabLeft, controller.SetWideZoom},
{Input.TabRight, controller.SetCloseZoom},
{Input.ZoomOut, controller.AdjustZoomOut},
{Input.ZoomIn, controller.AdjustZoomIn}
};
}

public override bool Press(Input input, PressType pressType)
{
return pressType switch
{
PressType.DelayedRepeat => DelayedRepeat(buttonMap[input], true),
PressType.InstantRepeat => InstantRepeat(buttonMap[input]),
PressType.Single => SinglePress(buttonMap[input], true),
_ => throw new ArgumentOutOfRangeException(nameof(pressType), pressType, null)
};
}

public override bool Peek(Input input, PressType pressType)
{
return pressType switch
{
PressType.DelayedRepeat => DelayedRepeat(buttonMap[input], false),
PressType.InstantRepeat => InstantRepeat(buttonMap[input]),
PressType.Single => SinglePress(buttonMap[input], false),
_ => throw new ArgumentOutOfRangeException(nameof(pressType), pressType, null)
};
}


public override bool Released(Input input)
{
return buttonMap[input].Released;
}
}
}
```

`InputKey.cs`

```cs
public class InputKey : GameControl
{
public static readonly IReadOnlyDictionary<Keys, KeyboardIcon> KeyIcons = new Dictionary<Keys, KeyboardIcon>
{
{Keys.A, KeyboardIcon.A},
{Keys.B, KeyboardIcon.B},
{Keys.C, KeyboardIcon.C},
{Keys.D, KeyboardIcon.D},
{Keys.E, KeyboardIcon.E},
{Keys.F, KeyboardIcon.F},
{Keys.G, KeyboardIcon.G},
{Keys.H, KeyboardIcon.H},
{Keys.I, KeyboardIcon.I},
{Keys.J, KeyboardIcon.J},
{Keys.K, KeyboardIcon.K},
{Keys.L, KeyboardIcon.L},
{Keys.M, KeyboardIcon.M},
{Keys.N, KeyboardIcon.N},
{Keys.O, KeyboardIcon.O},
{Keys.P, KeyboardIcon.P},
{Keys.Q, KeyboardIcon.Q},
{Keys.R, KeyboardIcon.R},
{Keys.S, KeyboardIcon.S},
{Keys.T, KeyboardIcon.T},
{Keys.U, KeyboardIcon.U},
{Keys.V, KeyboardIcon.V},
{Keys.W, KeyboardIcon.W},
{Keys.X, KeyboardIcon.X},
{Keys.Y, KeyboardIcon.Y},
{Keys.Z, KeyboardIcon.Z},

{Keys.D0, KeyboardIcon.Zero},
{Keys.D1, KeyboardIcon.One},
{Keys.D2, KeyboardIcon.Two},
{Keys.D3, KeyboardIcon.Three},
{Keys.D4, KeyboardIcon.Four},
{Keys.D5, KeyboardIcon.Five},
{Keys.D6, KeyboardIcon.Six},
{Keys.D7, KeyboardIcon.Seven},
{Keys.D8, KeyboardIcon.Eight},
{Keys.D9, KeyboardIcon.Nine},

{Keys.OemQuotes, KeyboardIcon.Apostrophe},
{Keys.OemPipe, KeyboardIcon.Backslash},
{Keys.Back, KeyboardIcon.Backspace},
{Keys.OemOpenBrackets, KeyboardIcon.BracketLeft},
{Keys.OemCloseBrackets, KeyboardIcon.BracketRight},
{Keys.OemComma, KeyboardIcon.Comma},
{Keys.OemPlus, KeyboardIcon.Equals},
{Keys.OemQuestion, KeyboardIcon.Forwardslash},
{Keys.OemMinus, KeyboardIcon.Minus},
{Keys.OemPeriod, KeyboardIcon.Period},
{Keys.OemSemicolon, KeyboardIcon.Semicolon},
{Keys.OemTilde, KeyboardIcon.Tilde},

{Keys.LeftShift, KeyboardIcon.LeftShift},
{Keys.LeftAlt, KeyboardIcon.LeftAlt},
{Keys.LeftControl, KeyboardIcon.LeftCtrl},
{Keys.RightShift, KeyboardIcon.RightShift},
{Keys.RightAlt, KeyboardIcon.RightAlt},
{Keys.RightControl, KeyboardIcon.RightCtrl},

{Keys.Space, KeyboardIcon.Space},
{Keys.Enter, KeyboardIcon.Enter},
{Keys.Escape, KeyboardIcon.Escape},
{Keys.Tab, KeyboardIcon.Tab},

{Keys.Up, KeyboardIcon.Up},
{Keys.Down, KeyboardIcon.Down},
{Keys.Left, KeyboardIcon.Left},
{Keys.Right, KeyboardIcon.Right},
};

private readonly Keys key;

public InputKey(Keys key)
{
this.key = key;
}
 public override bool Pressed => Keyboard.GetState().IsKeyDown(key);
```

`KeyboardController`

```cs
public class KeyboardController : IController
{
public static IController From(KeyboardController controller)
{
return new KeyboardController(
controller.Inputs[Input.Confirm],
controller.Inputs[Input.Cancel],
controller.Inputs[Input.PreviewUnit],
controller.Inputs[Input.PreviewItem],
controller.Inputs[Input.CursorUp],
controller.Inputs[Input.CursorDown],
controller.Inputs[Input.CursorLeft],
controller.Inputs[Input.CursorRight],
controller.Inputs[Input.CameraUp],
controller.Inputs[Input.CameraDown],
controller.Inputs[Input.CameraLeft],
controller.Inputs[Input.CameraRight],
controller.Inputs[Input.Menu],
controller.Inputs[Input.Status],
controller.Inputs[Input.TabLeft],
controller.Inputs[Input.TabRight],
controller.Inputs[Input.ZoomOut],
controller.Inputs[Input.ZoomIn]
);
}

public Dictionary<Input, GameControl> Inputs { get; }

public ControlType ControlType => ControlType.Keyboard;

public GameControl Confirm => Inputs[Input.Confirm];
public GameControl Cancel => Inputs[Input.Cancel];
public GameControl PreviewUnit => Inputs[Input.PreviewUnit];
public GameControl PreviewItem => Inputs[Input.PreviewItem];

public GameControl CursorUp => Inputs[Input.CursorUp];
public GameControl CursorDown => Inputs[Input.CursorDown];
public GameControl CursorLeft => Inputs[Input.CursorLeft];
public GameControl CursorRight => Inputs[Input.CursorRight];

public GameControl CameraUp => Inputs[Input.CameraUp];
public GameControl CameraDown => Inputs[Input.CameraDown];
public GameControl CameraLeft => Inputs[Input.CameraLeft];
public GameControl CameraRight => Inputs[Input.CameraRight];

public GameControl Menu => Inputs[Input.Menu];
public GameControl Status => Inputs[Input.Status];

public GameControl SetWideZoom => Inputs[Input.TabLeft];
public GameControl SetCloseZoom => Inputs[Input.TabRight];
public GameControl AdjustZoomOut => Inputs[Input.ZoomOut];
public GameControl AdjustZoomIn => Inputs[Input.ZoomIn];
```

### Load

A class `Content Loader` carrega o conteúdo que se encontra nos Assets. E até apresenta algumas músicas Originais sobre a forma comentário

`ContentLoader.cs`

```cs
public static List<ITexture2D> LoadUnitSpriteTextures(ContentManager content)
{
var loadSpriteTextures = new List<Texture2D>
{
content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueArcher"),
content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueMage"),
content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueChampion"),
content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueBard"),
content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueLancer"),
content.Load<Texture2D>("Graphics/Map/Units/Blue/BluePugilist"),
content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueDuelist"),
content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueCleric"),
content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueMarauder"),
content.Load<Texture2D>("Graphics/Map/Units/Blue/BluePaladin"),
content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueCavalier"),
content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueRogue"),
content.Load<Texture2D>("Graphics/Map/Units/Blue/BlueBoar"),

content.Load<Texture2D>("Graphics/Map/Units/Red/RedArcher"),
content.Load<Texture2D>("Graphics/Map/Units/Red/RedMage"),
content.Load<Texture2D>("Graphics/Map/Units/Red/RedChampion"),
content.Load<Texture2D>("Graphics/Map/Units/Red/RedBard"),
content.Load<Texture2D>("Graphics/Map/Units/Red/RedLancer"),
content.Load<Texture2D>("Graphics/Map/Units/Red/RedPugilist"),
content.Load<Texture2D>("Graphics/Map/Units/Red/RedDuelist"),
content.Load<Texture2D>("Graphics/Map/Units/Red/RedCleric"),
content.Load<Texture2D>("Graphics/Map/Units/Red/RedMarauder"),
content.Load<Texture2D>("Graphics/Map/Units/Red/RedPaladin"),
content.Load<Texture2D>("Graphics/Map/Units/Red/RedCavalier"),
content.Load<Texture2D>("Graphics/Map/Units/Red/RedRogue"),
content.Load<Texture2D>("Graphics/Map/Units/Red/RedBoar"),

content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepSlime"),
content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepTroll"),
content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepOrc"),
content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepBloodOrc"),
content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepKobold"),
content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepDragon"),
content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepMerchant"),
content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepNecromancer"),
content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepSkeleton"),
content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepGoblin"),
content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepRat"),
content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepBat"),
content.Load<Texture2D>("Graphics/Map/Units/Creep/CreepSpider")
};

return loadSpriteTextures.Select(texture => new Texture2DWrapper(texture)).Cast<ITexture2D>().ToList();
}
```

```cs
//Original (High Quality)
//new SoundEffectWrapper(content.Load<SoundEffect>("Audio/Music/Game/MapSelectTheme"), SongSFXVolume),
//new SoundEffectWrapper(content.Load<SoundEffect>("Audio/Music/Game/DarkTheme"), SongSFXVolume),
//new SoundEffectWrapper(content.Load<SoundEffect>("Audio/Music/Game/DungeonTheme"), SongSFXVolume),
//new SoundEffectWrapper(content.Load<SoundEffect>("Audio/Music/Game/TacticalTheme"), SongSFXVolume),
//new SoundEffectWrapper(content.Load<SoundEffect>("Audio/Music/Game/GallopTheme"), SongSFXVolume),
//new SoundEffectWrapper(content.Load<SoundEffect>("Audio/Music/Game/RegularBattle"), SongSFXVolume),
//new SoundEffectWrapper(content.Load<SoundEffect>("Audio/Music/Game/CaveTheme"), SongSFXVolume),
//new SoundEffectWrapper(content.Load<SoundEffect>("Audio/Music/Game/JazzBattle"), SongSFXVolume),
//new SoundEffectWrapper(content.Load<SoundEffect>("Audio/Music/Game/VictoryJingle"), SongSFXVolume),
```

### Eventos

A primeira classe AddItemToUnitInventoryEvent permite adicionar um item ao inventário. Quando esta ação é concluída, é exibida uma notificação e é reproduzido um efeito sonoro usando a classe AssetManager.
A segunda classe BankDepositEvent representa um evento de depositar ouro num banco. 
A terceira classe EndTurnEvent representa um evento de encerrar a ronda num jogo. A classe Duelist começa uma nova ação antes do fim da ronda. 
A quarta classe SpawnCreepEvent gera um monstro e coloca-o numa tile.

`AddItemsToInventoryEvent.cs`

```cs
public class AddItemToUnitInventoryEvent : IEvent
{
private readonly GameUnit unit;
private readonly IItem item;
public bool Complete { get; private set; }

public AddItemToUnitInventoryEvent(GameUnit unit, IItem item)
{
this.unit = unit;
this.item = item;
}

public void Continue()
{
unit.AddItemToInventory(item);

ItemToast(unit, item);
AssetManager.SkillBuffSFX.Play();
Complete = true;
}

public static void ItemToast(GameUnit unit, IItem item)
{
IRenderable itemToast = new WindowContentGrid(
new[,]
{
{
SpriteResizer.TryResizeRenderable(item.Icon, new Vector2(MapContainer.MapToastIconSize)),
new RenderText(AssetManager.MapFont, unit.Id + " got " + item.Name + "!")
}
}
);

GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(itemToast, 50);
}
}
```

`BankDepositEvent.cs`

```cs
public class BankDepositEvent : IEvent
{
private readonly GameUnit actingUnit;
private readonly int goldToDeposit;

public BankDepositEvent(GameUnit actingUnit, int goldToDeposit)
{
this.actingUnit = actingUnit;
this.goldToDeposit = goldToDeposit;
}

public bool Complete { get; private set; }

public void Continue()
{
Bank.Deposit(actingUnit, goldToDeposit);
Complete = true;
}
}
```

`EndTurnEvent.cs`

```cs
public class EndTurnEvent : IEvent
{
public bool Complete { get; private set; }
private readonly bool duelistHasFocusPoints;

public EndTurnEvent()
{
duelistHasFocusPoints = FocusStatus.ActiveDuelistHasFocusPoints;
}

public void Continue()
{
if (duelistHasFocusPoints)
{
(GlobalContext.ActiveUnit.StatusEffects.Find(status => status is FocusStatus) as FocusStatus)
    ?.StartAdditionalAction();
}
//IMPORTANT Do not allow tiles that have been triggered to trigger again or the risk of soft-locking via infinite triggers can occur
else if (!WorldContext.TriggerEffectTiles(EffectTriggerTime.EndOfTurn, false))
{
WorldContext.FinishTurn(false);
}

Complete = true;
}
}
```

`Utility>Events>AI>SpawnCreepEvent.cs`

```cs
public class SpawnCreepEvent : IEvent
{
private readonly Role unitRole;
private readonly Dictionary<string, string> entityProperties;
private readonly Vector2 coordinates;
public bool Complete { get; private set; }

public SpawnCreepEvent(Role unitRole, Vector2 coordinates, Dictionary<string, string> entityProperties)
{
this.unitRole = unitRole;
this.entityProperties = entityProperties;
this.coordinates = coordinates;
}

public void Continue()
{
SummoningRoutine.PlaceCreepInTile(unitRole, coordinates, entityProperties);
Complete = true;
}
}
```

### Mapa

A classe MapCamera faz a implementação da interface IMapCamera e é responsável por controlar a câmara no mapa, permitindo zoom, pan e direções da câmara.

Já a classe MapCursor é responsável por representar o cursor do mapa, permitindo que o jogador interaja com o mapa. Contém informações sobre o tamanho do mapa, o tamanho do cursor e o sprite do cursor. Tem também um método para desenhar o cursor, incluindo alterar a sua cor e coordenadas. Além disso, permite determinar se o cursor está dentro de uma janela e desenhar botões de prompt em diferentes estados de jogo.

`MapCamera.cs`

```cs
public enum CameraDirection
{
Up,
Right,
Down,
Left
}


public class MapCamera : IMapCamera
{
private static readonly Dictionary<IMapCamera.ZoomLevel, float> ZoomLevels =
new Dictionary<IMapCamera.ZoomLevel, float>
{
{IMapCamera.ZoomLevel.Far, FarZoom},
{IMapCamera.ZoomLevel.Default, DefaultZoomLevel},
{IMapCamera.ZoomLevel.Close, CloseZoom},
{IMapCamera.ZoomLevel.Combat, CombatZoom}
};

private const double FloatTolerance = 0.01;

private const float FarZoom = 1.4f;
private const float DefaultZoomLevel = 2;
private const float CloseZoom = 3;
private const float CombatZoom = 4;

private const int TopCursorThreshold = 250;
private const int HorizontalCursorThreshold = 300;
private const int BottomCursorThreshold = 300;

public float CurrentZoom { get; private set; }
public float TargetZoom { get; private set; }
private readonly float zoomRate;

private Vector2 currentPosition;
private Vector2 targetPosition;
private readonly float panRate;
private bool movingCameraToCursor;

private bool centeringOnPoint;
private float lastZoom;

public MapCamera(float panRate, float zoomRate)
{
currentPosition = new Vector2(0);
targetPosition = new Vector2(0);
CurrentZoom = DefaultZoomLevel;
TargetZoom = DefaultZoomLevel;
lastZoom = DefaultZoomLevel;
centeringOnPoint = false;
this.panRate = panRate;
this.zoomRate = zoomRate;
}




public void UpdateEveryFrame()
{
if (targetPosition == currentPosition && Math.Abs(TargetZoom - CurrentZoom) < FloatTolerance)
{
centeringOnPoint = false;
}

if (centeringOnPoint)
{
CenterCameraToCursor();
PanCameraToTarget(1 / zoomRate);
}
else
{
PanCameraToTarget(panRate);
}

UpdateZoomLevel();
UpdateCameraToCursor();
CorrectCameraToMap();
}



private void UpdateZoomLevel()
{
//Too big; zoom out
if (CurrentZoom > TargetZoom)
{
if (CurrentZoom - zoomRate < TargetZoom)
{
CurrentZoom = TargetZoom;
return;
}

CurrentZoom -= zoomRate;
}

//Too small; zoom in
if (CurrentZoom < TargetZoom)
{
if (CurrentZoom + zoomRate > TargetZoom)
{
CurrentZoom = TargetZoom;
return;
}

CurrentZoom += zoomRate;
}
}
```

`MapCursor.cs`

```cs
private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

private enum CursorColor
{
White,
Blue,
Red
}


private const int ButtonIconSize = 16;
private readonly Vector2 mapSize;
private static Vector2 _cursorSize;
public Vector2 CenterPixelPoint => CurrentDrawCoordinates + (new Vector2(Sprite.Width, Sprite.Height) / 2);
public Vector2 CenterTargetPixelPoint => MapPixelCoordinates + (new Vector2(Sprite.Width, Sprite.Height) / 2);
private SpriteAtlas SpriteAtlas => (SpriteAtlas) Sprite;

public Vector2 CenterCursorScreenCoordinates =>
(CurrentDrawCoordinates + (_cursorSize / 2) + GlobalContext.MapCamera.TargetPosition) *
GlobalContext.MapCamera.TargetZoom;

// ReSharper disable once SuggestBaseTypeForParameter
public MapCursor(SpriteAtlas sprite, Vector2 mapCoordinates, Vector2 mapSize) : base(sprite, mapCoordinates)
{
this.mapSize = mapSize;
_cursorSize = new Vector2(sprite.Width, sprite.Height);
}



public bool CursorIntersectsWindow(IRenderable window, Vector2 windowPixelPosition)
{
(float windowLeft, float windowTop) = windowPixelPosition;
float windowRight = windowLeft + window.Width;
float windowBottom = windowTop + window.Height;
(float cursorX, float cursorY) = CenterCursorScreenCoordinates;

bool cursorWithinWindowBounds = (cursorX >= windowLeft && cursorX <= windowRight) &&
(cursorY >= windowTop && cursorY <= windowBottom);
return cursorWithinWindowBounds;
}

public override void Draw(SpriteBatch spriteBatch)
{
Draw(spriteBatch, Color.White);
}

protected override void Draw(SpriteBatch spriteBatch, Color colorOverride)
{
UpdateCursorTeam();
UpdateRenderCoordinates();
Sprite.Draw(spriteBatch, CurrentDrawCoordinates, colorOverride);

switch (GlobalContext.CurrentGameState)
{
case GlobalContext.GameState.EULAConfirm:
break;
case GlobalContext.GameState.MainMenu:
break;
case GlobalContext.GameState.NetworkMenu:
break;
case GlobalContext.GameState.ArmyDraft:
break;
case GlobalContext.GameState.Deployment:
DrawDeploymentButtonPrompts(spriteBatch);
break;
case GlobalContext.GameState.MapSelect:
DrawMapSelectButtonPrompts(spriteBatch);
break;
case GlobalContext.GameState.PauseScreen:
break;
case GlobalContext.GameState.InGame:
DrawInGameButtonPrompts(spriteBatch);
break;
case GlobalContext.GameState.Results:
break;
case GlobalContext.GameState.Codex:
break;
case GlobalContext.GameState.ItemPreview:
break;
case GlobalContext.GameState.Credits:
break;
case GlobalContext.GameState.ControlConfig:
break;
case GlobalContext.GameState.HowToPlay:
break;
default:
throw new ArgumentOutOfRangeException();
}
}
```

### Entidades

A classe BasicAttack.cs representa a ação básica de ataque de uma personagem num jogo de estratégia baseado em rondas. A função ExecuteAction é responsável pelo ataque a um determinado alvo. O parâmetro targetSlice representa uma fatia do mapa onde o alvo está localizado. O alvo é selecionado usando a função SelectUnit do objeto UnitSelector e é verificado se está dentro do alcance de ataque dessa personagem. Se estiver, ele adiciona um evento de combate à queue do jogo usando o objeto GlobalEventQueue. Se o alvo for um obstáculo destrutível, ele chama a função DamageTerrain para causar dano ao obstáculo e adiciona eventos de espera e fim da ronda à queue. Caso contrário, é exibida uma mensagem de erro. A função DamageTerrain é responsável por causar dano a um obstáculo destrutível. O parâmetro targetSlice representa a fatia do mapa onde o obstáculo está localizado. A função usa o objeto targetObstacle para aceder à entidade do obstáculo e chama a função DealDamage para causar dano. Se o obstáculo for destruído, a função remove a entidade do obstáculo do mapa usando o objeto MapContainer.

A classe HealthRegeneration.cs representa o efeito de regeneração de vida. São definidos os parâmetros do efeito tal como a duração, o ícone, o nome, a descrição e outras propriedades do mesmo. A função ExecuteEffect é responsável por executar a regeneração de vida. É exibida uma notificação e é reproduzido um efeito sonoro.

`BasicAttack.cs`

```cs
public override void ExecuteAction(MapSlice targetSlice)
{
GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
{
GlobalEventQueue.QueueSingleEvent(new StartCombatEvent(targetUnit));
}
else if (TargetIsABreakableObstacleInRange(targetSlice))
{
DamageTerrain(targetSlice);

GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(10));
GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());
}
else
{
GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't attack here!", 50);
AssetManager.WarningSFX.Play();
}
}

public static void DamageTerrain(MapSlice targetSlice)
{
var targetObstacle = (BreakableObstacle) targetSlice.TerrainEntity;
targetObstacle.DealDamage(1);

if (targetObstacle.IsBroken)
{
MapContainer.GameGrid[(int) Layer.Entities]
[(int) targetObstacle.MapCoordinates.X, (int) targetObstacle.MapCoordinates.Y] = null;
}
}
```

`HealthRegeneration.cs`

```cs
public HealthRegeneration(int turnDuration, int healthModifier) : base(
statusIcon: StatusIconProvider.GetStatusIcon(Utility.Assets.StatusIcon.HpUp,
GameDriver.CellSizeVector),
name: UnitStatistics.Abbreviation[Stats.Hp] + " Regen! <+" + healthModifier + "/turn>",
description: "Increased defensive power.",
turnDuration: turnDuration,
hasNotification: true,
canCleanse: false
)


protected override void ExecuteEffect(GameUnit target)
{
target.RecoverHP(healthModifier);

GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(
target.UnitEntity,
target.Id + " regenerates [" + healthModifier + "] " + UnitStatistics.Abbreviation[Stats.Hp] + "!",
50
);

AssetManager.SkillBuffSFX.Play();
}
```

### Menu

A classe TwoDimensionalMenu.cs define um menu que pode ser navegável com um cursor. A classe possui campos e propriedades que armazenam informações sobre a janela do menu, a posição e tipo do cursor, as opções do menu, a linha e coluna da opção atualmente selecionada, bem como métodos para construir e desenhar o menu.

A interface IMenu.cs define um conjunto de métodos e propriedades que uma classe de menu deve implementar, incluindo a capacidade de mover o cursor em todas as direções, selecionar uma opção e obter a opção atualmente selecionada. Essa interface é relevante para garantir consistência nas funcionalidade do jogo.

`TwoDimensionalMenu.cs`

```cs
public enum CursorType
{
Pointer,
Frame
}

private const int ButtonIconSize = 32;
private const int Padding = 2;

private readonly Window.Window menuWindow;
private readonly IRenderable cursorSprite;
private Vector2 cursorPosition;
private readonly CursorType cursorType;
private readonly MenuOption[,] options;
private Vector2 optionSize;

private int CurrentOptionRow { get; set; }
private int CurrentOptionColumn { get; set; }
public Color DefaultColor { get; set; }
public bool IsVisible { get; set; }


private Window.Window BuildMenuWindow()
{
EqualizeOptionSizes(options);

// ReSharper disable once CoVariantArrayConversion
var menuContent = new WindowContentGrid(options, Padding, HorizontalAlignment.Centered);

return new Window.Window(menuContent, DefaultColor);
}


public MenuOption CurrentOption => options[CurrentOptionRow, CurrentOptionColumn];

public void SelectOption()
{
CurrentOption.Execute();
AssetManager.MenuConfirmSFX.Play();
}



public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
{
if (!IsVisible) return;
menuWindow.Draw(spriteBatch, position, colorOverride);

Color cursorColor = TeamUtility.DetermineTeamCursorColor(GlobalContext.ActiveTeam);
cursorSprite.Draw(spriteBatch, position + cursorPosition, cursorColor);

ConfirmButton.Draw(spriteBatch,
position + cursorPosition +
(
(cursorType == CursorType.Pointer)
? CenterLeftOffset(ConfirmButton, cursorSprite) + IconOffsetHack
: BottomRightCornerInset(ConfirmButton, cursorSprite)
)
);
}
```

`IMenu.cs`

```cs
namespace SolStandard.HUD.Menu
{
public enum MenuCursorDirection
{
Up,
Down,
Left,
Right
}

public interface IMenu : IRenderable
{
void MoveMenuCursor(MenuCursorDirection direction);
void SelectOption();
MenuOption CurrentOption { get; }
bool IsVisible { get; set; }
}
}
```
# ProjetoTDJD
