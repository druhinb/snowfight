static class EngineModifications{
    public static Vector2 mousePos = Engine.MousePosition, mouseMotion = Vector2.Zero;
    static EngineModifications(){}

    public static void updatePerFrameEngine() { calculateMouseMotion(); }

    public static void calculateMouseMotion()
    {
        mouseMotion = Engine.MousePosition - mousePos;
        mousePos = Engine.MousePosition;
    }

    public static Vector2 getMouseMotion() { return mouseMotion; }
}