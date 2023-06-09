﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Map.Camera
{
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

        public Vector2 CurrentPosition => currentPosition;

        public Vector2 TargetPosition => targetPosition;

        public Matrix CameraMatrix =>
            Matrix.CreateTranslation((int) currentPosition.X, (int) currentPosition.Y, 0) *
            Matrix.CreateScale(new Vector3(CurrentZoom, CurrentZoom, 1));

        public void RevertToPreviousZoomLevel()
        {
            if (Math.Abs(TargetZoom - lastZoom) > FloatTolerance)
            {
                ZoomToCursor(lastZoom);
            }
        }

        public void SetZoomLevel(IMapCamera.ZoomLevel zoomLevel)
        {
            ZoomToCursor(ZoomLevels[zoomLevel]);
        }

        public void ZoomIn()
        {
            if (CurrentZoom >= ZoomLevels[IMapCamera.ZoomLevel.Close]) return;

            if (CurrentZoom >= ZoomLevels[IMapCamera.ZoomLevel.Default]) SetZoomLevel(IMapCamera.ZoomLevel.Close);
            else if (CurrentZoom >= ZoomLevels[IMapCamera.ZoomLevel.Far]) SetZoomLevel(IMapCamera.ZoomLevel.Default);
        }

        public void ZoomOut()
        {
            if (CurrentZoom <= ZoomLevels[IMapCamera.ZoomLevel.Far]) return;

            if (CurrentZoom <= ZoomLevels[IMapCamera.ZoomLevel.Default]) SetZoomLevel(IMapCamera.ZoomLevel.Far);
            else if (CurrentZoom <= ZoomLevels[IMapCamera.ZoomLevel.Close]) SetZoomLevel(IMapCamera.ZoomLevel.Default);
            else if (CurrentZoom <= ZoomLevels[IMapCamera.ZoomLevel.Combat]) SetZoomLevel(IMapCamera.ZoomLevel.Close);
        }

        private void ZoomToCursor(float zoomLevel)
        {
            if (Math.Abs(TargetZoom - zoomLevel) > FloatTolerance)
            {
                lastZoom = TargetZoom;
                TargetZoom = zoomLevel;
            }

            centeringOnPoint = true;
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

        public void SnapCameraCenterToCursor()
        {
            CenterCameraToPoint(GlobalContext.MapCursor.CenterPixelPoint);
            currentPosition = targetPosition;
        }

        public void CenterCameraToCursor()
        {
            CenterCameraToPoint(GlobalContext.MapCursor.CenterPixelPoint);
        }

        private void CenterCameraToPoint(Vector2 centerPoint)
        {
            Vector2 screenCenter = GameDriver.ScreenSize / 2;

            targetPosition = Vector2.Negate(centerPoint);
            targetPosition += screenCenter / CurrentZoom;
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

        private void PanCameraToTarget(float panSpeed)
        {
            if (currentPosition.X < targetPosition.X)
            {
                if (currentPosition.X + panSpeed > targetPosition.X)
                {
                    currentPosition.X = targetPosition.X;
                }
                else
                {
                    currentPosition.X += panSpeed;
                }
            }

            if (currentPosition.X > targetPosition.X)
            {
                if (currentPosition.X - panSpeed < targetPosition.X)
                {
                    currentPosition.X = targetPosition.X;
                }
                else
                {
                    currentPosition.X -= panSpeed;
                }
            }

            if (currentPosition.Y < targetPosition.Y)
            {
                if (currentPosition.Y + panSpeed > targetPosition.Y)
                {
                    currentPosition.Y = targetPosition.Y;
                }
                else
                {
                    currentPosition.Y += panSpeed;
                }
            }

            if (currentPosition.Y > targetPosition.Y)
            {
                if (currentPosition.Y - panSpeed < targetPosition.Y)
                {
                    currentPosition.Y = targetPosition.Y;
                }
                else
                {
                    currentPosition.Y -= panSpeed;
                }
            }
        }

        private void MoveCameraInDirection(CameraDirection direction)
        {
            MoveCameraInDirection(direction, panRate);
        }

        public void MoveCameraInDirection(CameraDirection direction, float panRateOverride)
        {
            switch (direction)
            {
                case CameraDirection.Down:
                    targetPosition.Y -= panRateOverride;
                    break;
                case CameraDirection.Right:
                    targetPosition.X -= panRateOverride;
                    break;
                case CameraDirection.Up:
                    targetPosition.Y += panRateOverride;
                    break;
                case CameraDirection.Left:
                    targetPosition.X += panRateOverride;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public void StopMovingCamera()
        {
            targetPosition = currentPosition;
        }

        public void StartMovingCameraToCursor()
        {
            movingCameraToCursor = true;
        }

        private void UpdateCameraToCursor()
        {
            if (movingCameraToCursor)
            {
                if (GlobalContext.MapCursor.CenterCursorScreenCoordinates.X < WestBound)
                {
                    MoveCameraInDirection(CameraDirection.Left);
                }

                if (GlobalContext.MapCursor.CenterCursorScreenCoordinates.X > EastBound)
                {
                    MoveCameraInDirection(CameraDirection.Right);
                }

                if (GlobalContext.MapCursor.CenterCursorScreenCoordinates.Y < NorthBound)
                {
                    MoveCameraInDirection(CameraDirection.Up);
                }

                if (GlobalContext.MapCursor.CenterCursorScreenCoordinates.Y > SouthBound)
                {
                    MoveCameraInDirection(CameraDirection.Down);
                }
            }

            if (targetPosition == currentPosition)
            {
                movingCameraToCursor = false;
            }
        }

        private float WestBound =>
            0 + HorizontalCursorThreshold +
            (GlobalContext.MapCursor.RenderSprite.Width * CurrentZoom);

        private float EastBound =>
            GameDriver.ScreenSize.X - HorizontalCursorThreshold -
            (GlobalContext.MapCursor.RenderSprite.Width * CurrentZoom);

        private float NorthBound => 0 + TopCursorThreshold;

        private float SouthBound =>
            GameDriver.ScreenSize.Y - BottomCursorThreshold -
            (GlobalContext.MapCursor.RenderSprite.Height * CurrentZoom);

        private void CorrectCameraToMap()
        {
            CorrectPositionToMap(MapContainer.MapGridSize, currentPosition);
        }

        private void CorrectPositionToMap(Vector2 mapSize, Vector2 position)
        {
            if (CameraIsBeyondLeftEdge(position) && CameraIsBeyondRightEdge(mapSize, position))
            {
                CenterHorizontally();
            }
            else
            {
                if (CameraIsBeyondLeftEdge(position))
                {
                    currentPosition.X = 0;
                    targetPosition.X = currentPosition.X;

                    //If new position would be beyond Right Edge, just center
                    if (CameraIsBeyondRightEdge(mapSize, currentPosition)) CenterHorizontally();
                }
                else if (CameraIsBeyondRightEdge(mapSize, position))
                {
                    currentPosition.X = RightEdge(mapSize) / CurrentZoom;
                    if (targetPosition.X < currentPosition.X)
                    {
                        targetPosition.X = currentPosition.X;
                    }

                    //If new position would be beyond Left Edge, just center
                    if (CameraIsBeyondLeftEdge(currentPosition)) CenterHorizontally();
                }
            }

            if (CameraIsBeyondTopEdge(position) && CameraIsBeyondBottomEdge(mapSize, position))
            {
                CenterVertically();
            }
            else
            {
                if (CameraIsBeyondTopEdge(position))
                {
                    currentPosition.Y = 0;
                    targetPosition.Y = currentPosition.Y;
                    //If new position would be beyond Bottom Edge, just center
                    if (CameraIsBeyondBottomEdge(mapSize, currentPosition)) CenterVertically();
                }
                else if (CameraIsBeyondBottomEdge(mapSize, position))
                {
                    currentPosition.Y = BottomEdge(mapSize) / CurrentZoom;
                    if (targetPosition.Y < currentPosition.Y)
                    {
                        targetPosition.Y = currentPosition.Y;
                    }

                    //If new position would be beyond Top Edge, just center
                    if (CameraIsBeyondTopEdge(currentPosition)) CenterVertically();
                }
            }
        }

        private void CenterVertically()
        {
            currentPosition.Y = (GameDriver.ScreenSize.Y - MapContainer.MapScreenSizeInPixels.Y) / 4;
            targetPosition.Y = currentPosition.Y;
        }

        private void CenterHorizontally()
        {
            currentPosition.X = (GameDriver.ScreenSize.X - MapContainer.MapScreenSizeInPixels.X) / 4;
            targetPosition.X = currentPosition.X;
        }

        private bool CameraIsBeyondBottomEdge(Vector2 mapSize, Vector2 position)
        {
            return position.Y * CurrentZoom < BottomEdge(mapSize);
        }

        private bool CameraIsBeyondRightEdge(Vector2 mapSize, Vector2 position)
        {
            return position.X * CurrentZoom < RightEdge(mapSize);
        }

        private bool CameraIsBeyondTopEdge(Vector2 position)
        {
            return position.Y > 0;
        }

        private bool CameraIsBeyondLeftEdge(Vector2 position)
        {
            return position.X > 0;
        }

        private float BottomEdge(Vector2 mapSize)
        {
            return (-1 * mapSize.Y * GameDriver.CellSize) * CurrentZoom + GameDriver.ScreenSize.Y;
        }

        private float RightEdge(Vector2 mapSize)
        {
            return (-1 * mapSize.X * GameDriver.CellSize) * CurrentZoom + GameDriver.ScreenSize.X;
        }
    }
}