using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.IO;

namespace Shader
{
    class shaders:GameWindow
    {
        int BasicProgramID; //Номер дескриптора на графической карте
        int BasicVertexShader; //Адрес вершинного шейдера  
        int BasicFragmentShader; //Адрес фрагментного шейдера
        int attribute_vcol; //Адрес параметра цвета
        int attribute_vpos; //Адрес параметра позиции
        int uniform_mview; //
        int vbo_position; //Адрес буфера вершин объекта для нашего параметра позиции
        int vbo_color; //Адрес буфера вершин объекта для нашего параметра цвета
        int vbo_mview; 
        Vector3[] vertdata; //Массив позиций вершин
        Vector3[] coldata; //Массив цветов вершин
        Matrix4[] mviewdata; //
        Vector3 campos;

        int uniform_aspect;
        int uniform_pos;
        float aspect;

        float angleX = 0.0f;
        float angleY = 0.0f;
        int mouseX = 0;
        int mouseY = 0;

        private void InitShaders()
        {
            // создание объекта программы 
            BasicProgramID = GL.CreateProgram();
            loadShader("..\\..\\basic.vs", ShaderType.VertexShader, BasicProgramID,
                       out BasicVertexShader);
            loadShader("..\\..\\basic.fs", ShaderType.FragmentShader, BasicProgramID,
                        out BasicFragmentShader);
            //Компановка программы
            GL.LinkProgram(BasicProgramID);

            // Проверить успех компановки
            int status = 0;
            GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out status);

            Console.WriteLine(GL.GetProgramInfoLog(BasicProgramID));

            attribute_vpos = GL.GetAttribLocation(BasicProgramID, "vPosition");
           // attribute_vcol = GL.GetAttribLocation(BasicProgramID, "vColor");
            //uniform_mview = GL.GetUniformLocation(BasicProgramID, "modelview");
            uniform_pos = GL.GetUniformLocation(BasicProgramID, "camp");
            uniform_aspect = GL.GetUniformLocation(BasicProgramID, "aspect");

            GL.GenBuffers(1, out vbo_position);   ///
            GL.GenBuffers(1, out vbo_color);      ///    Создание трёх буферных объектов
            GL.GenBuffers(1, out vbo_mview);      ///    Дескрипторы сохраняются для последующего использования
        }

        protected override void OnLoad(EventArgs e)   //Вызывает событие Load
        {
            base.OnLoad(e);

            InitShaders();

            vertdata = new Vector3[] { new Vector3(-1f, -1f, 0f),
                new Vector3( 1f, -1f, 0f), 
                new Vector3( 1f,  1f, 0f),
            new Vector3(-1f,1f,0f)};

                                                                         ///  Создали массивы в которых содержаться 
            coldata = new Vector3[] { new Vector3(1f, 1f, 0f),           ///  параметры вершин
                new Vector3( 1f, 1f, 0f), 
                new Vector3( 1f,  1f, 0f)};

            mviewdata = new Matrix4[]{
                Matrix4.Identity
            };

            campos = new Vector3(1, 2, 5);
        }

        void loadShader(String filename, ShaderType type, int program, out int address)
        {

            address = GL.CreateShader(type);  //Создает объект шрейдера с одним из типов
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());    // Загружает исходный код в созданный шейдерный объект
            } 
            GL.CompileShader(address);  // Компиляция шейдера
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        protected override void OnRenderFrame(FrameEventArgs e)   //Отвечает за перересовку
        {
            base.OnRenderFrame(e);
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);  //Очищать буфер
            GL.Enable(EnableCap.DepthTest);    //Дальние элементы перекрываются ближними

            campos = new Vector3((float)Math.Sin(angleX / 180 * Math.PI), (float)Math.Tan(angleY / 180 * Math.PI), 
                (float)Math.Cos(angleY / 180 * Math.PI));


            GL.EnableVertexAttribArray(attribute_vpos);   //Активация атрибутов вершин
            GL.EnableVertexAttribArray(attribute_vcol);   //Или вкл. режима отрисовки

            GL.DrawArrays(PrimitiveType.Quads,0, 4);  //Отображает нашу фигуру

            GL.DisableVertexAttribArray(attribute_vpos);  //Выключаем режим отрисовки
            GL.DisableVertexAttribArray(attribute_vcol);

            SwapBuffers();   //Быстро скопировать содержимое заднего буфера окна в передний буфер
        }

        protected override void OnUpdateFrame(FrameEventArgs e)   //Отвечает за обновление Update
        {
            base.OnUpdateFrame(e);

            var mouse = OpenTK.Input.Mouse.GetState();
            mouseX = mouse.X;
            mouseY = mouse.Y;
            int xc = Width / 2;
            int yc = Height / 2;
            angleX = (xc - mouseX) / 8;
            angleY = (yc - mouseY) / 8;

            if (angleY < -89.0f)
                angleY = -89.0f;
            if (angleY > 89.0f)
                angleY = 89.0f;




            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(vertdata.Length * Vector3.SizeInBytes), vertdata, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);

            /*GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_color);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(coldata.Length * Vector3.SizeInBytes), coldata, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vcol, 3, VertexAttribPointerType.Float, true, 0, 0);*/

            GL.UniformMatrix4(uniform_mview, false, ref mviewdata[0]);

            GL.UseProgram(BasicProgramID);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.Uniform3(uniform_pos, campos);
        }

    }
}
