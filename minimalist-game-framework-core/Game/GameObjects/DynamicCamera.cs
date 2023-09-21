using System;
using System.Collections.Generic;

// Tile-based moveable camera
public class DynamicCamera
{
    // Essential configuration variables
    // Minimum and maximum x and y values
    private Tuple<int, int> minCoords, maxCoords;
    private int displayHalfWidth, displayHalfHeight;

    // How many pixels per unit
    private int scale;

    // How fast the camera accelerates to maximum speed
    private float acceleration;

    // Increases deceleration with velocity - like air resistance
    private float resistance;

    // Deceleration with no input
    private float friction;

    // Position / velocity vectors for the camera object
    private Vector2 position;
    private Vector2 velocity;

    /// <summary>
    /// Creates a moveable camera.
    /// </summary>
    /// <param name="position">Starting position. (x, y)</param>
    /// <param name="acceleration">Acceleration of arrow keys. (units/second/second)</param>
    /// <param name="resistance">Resistance to acceleration. (0 < resistance < 1) </param>
    /// <param name="minIndices">Minimum X and Y tile indices. (minX, minY)</param>
    /// <param name="maxIndices">Maximum X and Y tile indices. (maxX, maxY)</param>
    /// <param name="scale">Size of each tile.</param>
    /// <param name="displayHalf">Half of the width and height in pixels. (halfWidth, halfHeight)</param>
    public DynamicCamera(Vector2 position, float acceleration, float resistance, Tuple<int, int> minIndices,
        Tuple<int, int> maxIndices, int scale, Tuple<int, int> displayHalf)
    {
        this.position = position;
        this.acceleration = acceleration * 2;
        this.resistance = resistance;
        this.minCoords = minIndices;
        this.maxCoords = maxIndices;
        this.scale = scale;
        this.displayHalfWidth = displayHalf.Item1;
        this.displayHalfHeight = displayHalf.Item2;

        this.velocity = Vector2.Zero;
    }

    // get methods
    public int getScale() { return scale; }
    public Vector2 getPosition() { return position; }
    public Vector2 getVelocity() { return velocity; }

    // set methods
    public void setScale(int newScale)
    {
        position /= scale;
        scale = newScale;
        position *= scale;
    }
    public void changeScale(int dScale) { setScale(scale + dScale); }
    public void setPosition(Vector2 position) { this.position = position; }

    // Handle user input (arrow keys) in order to update camera position
    public void handleInput()
    {
        if (Engine.GetKeyDown(Key.Equals) || Engine.GetKeyHeld(Key.Equals))
            changeScale(scale + 2 < 128 ? 2 : 0);

        if (Engine.GetKeyDown(Key.Minus) || Engine.GetKeyHeld(Key.Minus))
            changeScale(scale - 2 > 16 ? -2 : 0);

        if (dragMove())
        {
            velocity = Vector2.Zero;
        }
        else
        {
            // Accelerate camera if arrow keys or WASD are held down
            if (Engine.GetKeyHeld(Key.Left) || Engine.GetKeyHeld(Key.Right) ||
                Engine.GetKeyHeld(Key.Up) || Engine.GetKeyHeld(Key.Down) ||
                Engine.GetKeyHeld(Key.W) || Engine.GetKeyHeld(Key.A) ||
                Engine.GetKeyHeld(Key.S) || Engine.GetKeyHeld(Key.D))
            {
                velocity += Engine.TimeDelta * acceleration *
                    new Vector2(((Engine.GetKeyHeld(Key.Right) || Engine.GetKeyHeld(Key.D)) ? 1 : 0) - ((Engine.GetKeyHeld(Key.Left) || Engine.GetKeyHeld(Key.A)) ? 1 : 0),
                    ((Engine.GetKeyHeld(Key.Down) || Engine.GetKeyHeld(Key.S)) ? 1 : 0) - ((Engine.GetKeyHeld(Key.Up) || Engine.GetKeyHeld(Key.W)) ? 1 : 0));
            }
        }

        // Constrain velocity to maximum and slow down camera if no input
        Vector2 velocityDecrease = Engine.TimeDelta * (new Vector2(velocity.X * velocity.X, velocity.Y * velocity.Y) * resistance +
            new Vector2(acceleration, acceleration) / 2);
        if (velocityDecrease.X > Math.Abs(velocity.X)) velocity.X = 0;
        else
        {
            if (velocity.X < 0) velocity.X += velocityDecrease.X;
            else velocity.X -= velocityDecrease.X;
        }
        if (velocityDecrease.Y > Math.Abs(velocity.Y)) velocity.Y = 0;
        else
        {
            if (velocity.Y < 0) velocity.Y += velocityDecrease.Y;
            else velocity.Y -= velocityDecrease.Y;
        }

        // Apply translation (as it should be for the frame)
        translate(velocity);
    }

    // Ensure camera does not go out of bounds
    private void checkPosition()
    {
        float minX = minCoords.Item1 * scale + displayHalfWidth, minY = minCoords.Item2 * scale + displayHalfHeight;
        float maxX = maxCoords.Item1 * scale - displayHalfWidth, maxY = maxCoords.Item2 * scale - displayHalfHeight;

        if (position.X <= minX || position.X >= maxX)
        {
            velocity.X = 0;
            if (position.X <= minX)
            {
                if (position.X >= maxX) position.X = 0;
                else position.X = minX;
            }
            else
                position.X = maxX;
        }
        if (position.Y <= minY || position.Y >= maxY)
        {
            velocity.Y = 0;
            if (position.Y <= minY)
            {
                if (position.Y >= maxY) position.Y = 0;
                else position.Y = minY;
            }
            else
                position.Y = maxY;
        }
    }

    private bool dragMove()
    {
        if (Engine.GetMouseButtonHeld(MouseButton.Right))
        {
            position -= EngineModifications.getMouseMotion();
            checkPosition();
            return true;
        }
        return false;
    }

    // Simply changes the position by adding a vector2
    private void translate(Vector2 change)
    {
        position += change;
        checkPosition();
    }

    public List<string> save()
    {
        List<string> s = new List<string>();

        Utilities.write(ref s, minCoords);
        Utilities.write(ref s, maxCoords);
        Utilities.write(ref s, displayHalfWidth);
        Utilities.write(ref s, displayHalfHeight);
        Utilities.write(ref s, scale);
        Utilities.write(ref s, acceleration);
        Utilities.write(ref s, resistance);
        Utilities.write(ref s, friction);
        Utilities.write(ref s, position);
        Utilities.write(ref s, velocity);

        return s;
    }

    public int load(ref List<string> s, int i)
    {
        Utilities.read(ref minCoords, s[i], s[i + 1]);
        i += 2;
        Utilities.read(ref maxCoords, s[i], s[i + 1]);
        i += 2;
        Utilities.read(ref displayHalfWidth, s[i++]);
        Utilities.read(ref displayHalfHeight, s[i++]);
        Utilities.read(ref scale, s[i++]);
        Utilities.read(ref acceleration, s[i++]);
        Utilities.read(ref resistance, s[i++]);
        Utilities.read(ref friction, s[i++]);
        Utilities.read(ref position, s[i], s[i + 1]);
        i += 2;
        Utilities.read(ref velocity, s[i], s[i + 1]);
        i += 2;

        return i;
    }
}