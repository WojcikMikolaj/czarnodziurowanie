﻿using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TemplateProject.ImGuiUtils;
using ShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace BlackHole;

public class Program : GameWindow
{
    public bool IsLoaded { get; private set; }
    
    private Shader shader;
    private ImGuiController controller;
    private Mesh rectangle;
    private Camera camera;
    private Texture texture;
    private TextureCube cubemap;
    private int _mass;
    private int _distance;

    public static void Main(string[] args)
    {
        using var program = new Program(GameWindowSettings.Default, NativeWindowSettings.Default);
        program.Title = "Project Title";
        program.Size = new Vector2i(1280, 800);
        program.Run();
    }

    public Program(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

    protected override void OnLoad()
    {
        base.OnLoad();

        shader = new Shader(("shader.vert", ShaderType.VertexShader), ("shader.frag", ShaderType.FragmentShader));
        controller = new ImGuiController(ClientSize.X, ClientSize.Y);

        camera = new Camera(new FirstPersonControl(), new PerspectiveView());

        float[] vertices = {
            1f,  1f, 1f,
            1f, -1f, 1f,
            -1f, -1f, 1f,
            -1f,  1f, 1f
        };
        float[] texCoords = {
            0.0f, 0.0f,
            0.0f, 1.0f,
            1.0f, 1.0f,
            1.0f, 0.0f
                
        };
        int[] indices= {
            0, 1, 3,
            1, 2, 3
        };  
        rectangle = new Mesh(PrimitiveType.Triangles, indices, (vertices, 0, 3), (texCoords, 1, 2));
        
        texture = new Texture("texture.jpg");

        cubemap = new TextureCube(new (string path, TextureTarget side)[
        ]
        {
            ("X-.png", TextureTarget.TextureCubeMapNegativeX),
            ("Z-.png", TextureTarget.TextureCubeMapNegativeZ),
            ("X+.png", TextureTarget.TextureCubeMapPositiveX),
            ("Y+.png", TextureTarget.TextureCubeMapPositiveY),
            ("Y-.png", TextureTarget.TextureCubeMapNegativeY),
            ("Z+.png", TextureTarget.TextureCubeMapPositiveZ),
        });
            
        GL.ClearColor(0.4f, 0.7f, 0.9f, 1.0f);
        GL.Disable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Lequal);

        IsLoaded = true;
    }

    protected override void OnUnload()
    {
        base.OnUnload();
            
        rectangle.Dispose();
        controller.Dispose();
        texture.Dispose();
        shader.Dispose();

        IsLoaded = false;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }
        
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
        controller.WindowResized(ClientSize.X, ClientSize.Y);
        camera.Aspect = (float) Size.X / Size.Y;
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        controller.Update(this, (float)args.Time);

        if(ImGui.GetIO().WantCaptureMouse) return;

        var keyboard = KeyboardState.GetSnapshot();
        var mouse = MouseState.GetSnapshot();
        
        camera.HandleInput(keyboard, mouse, (float) args.Time);
            
        if (keyboard.IsKeyDown(Keys.Escape)) Close();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
            
        GL.Disable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Lequal);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        shader.Use();
        cubemap.Use();
        
        shader.LoadInteger("cubemap", 0);
        shader.LoadMatrix4("projMatrix", camera.GetProjectionMatrix());
        shader.LoadMatrix4("invViewMatrix", camera.GetViewMatrix().Inverted());
        shader.LoadMatrix4("mvp", camera.GetProjectionViewMatrix());
        rectangle.Render();

        RenderGui();

        Context.SwapBuffers();
    }

    private void RenderGui()
    {
        ImGui.Begin("Parametry");
        if (ImGui.SliderInt("Masa", ref _mass, 1, 300))
        {
            if (_mass < 1)
            {
                _mass = 1;
            }

            if (_mass > 300)
            {
                _mass = 300;
            }
        }
        if(ImGui.SliderInt("Dystans", ref _distance, 1, 300))
        {
            if (_distance < 1)
            {
                _distance = 1;
            }

            if (_distance > 300)
            {
                _distance = 300;
            }
        }
        ImGui.End();
        controller.Render();
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);
            
        controller.PressChar((char)e.Unicode);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
            
        controller.MouseScroll(e.Offset);
    }
}