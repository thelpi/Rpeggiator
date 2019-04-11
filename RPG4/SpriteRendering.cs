using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RPG4
{
    internal static class SpriteRendering
    {
        private static Dictionary<SpriteContext, Brush> _spritesCaches = new Dictionary<SpriteContext, Brush>();

        internal static Brush GetRenderingBrush(RpeggiatorLib.Sprites.Sprite sprite)
        {
            SpriteContext spriteContext = new SpriteContext(sprite);

            if (!_spritesCaches.ContainsKey(spriteContext))
            {
                Brush brush = null;
                switch (spriteContext.Name)
                {
                    case nameof(RpeggiatorLib.Sprites.ActionnedArrow):
                        brush = BrushFromResourceName(nameof(Properties.Resources.Arrow));
                        break;
                    case nameof(RpeggiatorLib.Sprites.ActionnedBomb):
                        brush = BrushFromResourceName(nameof(Properties.Resources.Bomb));
                        break;
                    case nameof(RpeggiatorLib.Sprites.Chest):
                        if (spriteContext.OnOff == true)
                        {
                            brush = BrushFromResourceName(nameof(Properties.Resources.OpenChest));
                        }
                        else
                        {
                            brush = BrushFromResourceName(nameof(Properties.Resources.Chest));
                        }
                        break;
                    case nameof(RpeggiatorLib.Sprites.Enemy):
                        brush = BrushFromResourceName(nameof(Properties.Resources.Enemy));
                        break;
                    case nameof(RpeggiatorLib.Sprites.Pit):
                        brush = BrushFromResourceName(nameof(Properties.Resources.Pit));
                        break;
                    case nameof(RpeggiatorLib.Sprites.Player):
                        if (spriteContext.OnOff == true)
                        {
                            brush = BrushFromResourceName(nameof(Properties.Resources.PlayerRecovery));
                        }
                        else
                        {
                            brush = BrushFromResourceName(nameof(Properties.Resources.Player));
                        }
                        break;
                    case nameof(RpeggiatorLib.Sprites.WeaponHit):
                        brush = BrushFromResourceName(nameof(Properties.Resources.Sword));
                        break;
                    case nameof(RpeggiatorLib.Sprites.BombExplosion):
                        brush = new SolidColorBrush(Colors.Orange);
                        break;
                    case nameof(RpeggiatorLib.Sprites.Door):
                        brush = new SolidColorBrush(Colors.Brown);
                        break;
                    case nameof(RpeggiatorLib.Sprites.Floor):
                        brush = new SolidColorBrush(ColorFromFloorType(spriteContext.FloorType.Value));
                        break;
                    case nameof(RpeggiatorLib.Sprites.Gate):
                        brush = new SolidColorBrush(Colors.Sienna);
                        break;
                    case nameof(RpeggiatorLib.Sprites.GateTrigger):
                        if (spriteContext.OnOff == true)
                        {
                            brush = BrushFromResourceName(nameof(Properties.Resources.TriggerOn));
                        }
                        else
                        {
                            brush = BrushFromResourceName(nameof(Properties.Resources.TriggerOff));
                        }
                        break;
                    case nameof(RpeggiatorLib.Sprites.PermanentStructure):
                        brush = new SolidColorBrush(Colors.SlateGray);
                        break;
                    case nameof(RpeggiatorLib.Sprites.PickableItem):
                        brush = BrushFromItemType(spriteContext.ItemType);
                        break;
                    case nameof(RpeggiatorLib.Sprites.Rift):
                        brush = new SolidColorBrush(Colors.Silver);
                        break;
                    case nameof(RpeggiatorLib.Sprites.Screen):
                        brush = new SolidColorBrush(ColorFromFloorType(spriteContext.FloorType.Value));
                        break;
                    default:
                        throw new NotImplementedException();
                }

                _spritesCaches.Add(spriteContext, brush);
            }

            return _spritesCaches[spriteContext];
        }

        internal static Brush BrushFromItemType(RpeggiatorLib.Enums.ItemType? itemType)
        {
            Brush brush = null;
            switch (itemType)
            {
                case null:
                    brush = BrushFromResourceName(nameof(Properties.Resources.Coin));
                    break;
                case RpeggiatorLib.Enums.ItemType.Arrow:
                    brush = BrushFromResourceName(nameof(Properties.Resources.Arrow));
                    break;
                case RpeggiatorLib.Enums.ItemType.Bomb:
                    brush = BrushFromResourceName(nameof(Properties.Resources.Bomb));
                    break;
                case RpeggiatorLib.Enums.ItemType.Bow:
                    brush = BrushFromResourceName(nameof(Properties.Resources.Bow));
                    break;
                case RpeggiatorLib.Enums.ItemType.Lamp:
                    brush = BrushFromResourceName(nameof(Properties.Resources.Lamp));
                    break;
                case RpeggiatorLib.Enums.ItemType.LargeLifePotion:
                    brush = BrushFromResourceName(nameof(Properties.Resources.LifePotionLarge));
                    break;
                case RpeggiatorLib.Enums.ItemType.MediumLifePotion:
                    brush = BrushFromResourceName(nameof(Properties.Resources.LifePotionMedium));
                    break;
                case RpeggiatorLib.Enums.ItemType.SmallLifePotion:
                    brush = BrushFromResourceName(nameof(Properties.Resources.LifePotionSmall));
                    break;
                default:
                    throw new NotImplementedException();
            }
            return brush;
        }

        private static Color ColorFromFloorType(RpeggiatorLib.Enums.FloorType floorType)
        {
            switch (floorType)
            {
                case RpeggiatorLib.Enums.FloorType.Ground:
                    return Colors.Tan;
                case RpeggiatorLib.Enums.FloorType.Ice:
                    return Colors.Aquamarine;
                case RpeggiatorLib.Enums.FloorType.Lava:
                    return Colors.Red;
                case RpeggiatorLib.Enums.FloorType.Water:
                    return Colors.Aqua;
                default:
                    throw new NotImplementedException();
            }
        }

        private static Brush BrushFromResourceName(string resourceName)
        {
            return new ImageBrush
            {
                ImageSource = FromBitmap(resourceName),
                Stretch = Stretch.Fill
            };
        }

        private static BitmapImage FromBitmap(string imagePath)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream memory = new MemoryStream())
            {
                System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)Properties.Resources.ResourceManager.GetObject(imagePath);
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }
    }

    internal struct SpriteContext
    {
        internal string Name { get; private set; }
        internal bool? OnOff { get; private set; }
        internal RpeggiatorLib.Enums.FloorType? FloorType { get; private set; }
        internal RpeggiatorLib.Enums.ItemType? ItemType { get; private set; }

        public SpriteContext(RpeggiatorLib.Sprites.Sprite sprite)
        {
            Name = sprite.GetType().Name;
            OnOff = null;
            FloorType = null;
            ItemType = null;
            switch (Name)
            {
                case nameof(RpeggiatorLib.Sprites.Chest):
                    OnOff = (sprite as RpeggiatorLib.Sprites.Chest).IsOpen;
                    break;
                case nameof(RpeggiatorLib.Sprites.Player):
                    OnOff = (sprite as RpeggiatorLib.Sprites.Player).IsRecovering;
                    break;
                case nameof(RpeggiatorLib.Sprites.Floor):
                case nameof(RpeggiatorLib.Sprites.Screen):
                    FloorType = (sprite as RpeggiatorLib.Sprites.Floor).FloorType;
                    break;
                case nameof(RpeggiatorLib.Sprites.GateTrigger):
                    OnOff = (sprite as RpeggiatorLib.Sprites.GateTrigger).IsActivated;
                    break;
                case nameof(RpeggiatorLib.Sprites.PickableItem):
                    ItemType = (sprite as RpeggiatorLib.Sprites.PickableItem).ItemType;
                    break;
            }
        }
    }
}
