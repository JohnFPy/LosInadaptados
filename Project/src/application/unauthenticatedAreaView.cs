﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Project.infrastucture;
using Project.infrastucture.utils;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Project.domain;
using Project.presentation.Views.AuthViews;
using System;
using System.Drawing.Printing;

namespace Project.presentation.Views.UnauthViews
{
    public partial class UnauthenticatedAreaView : UserControl
    {
        private Grid? _registerGrid;
        private Grid? _loginGrid;

        // Register TextBoxes & Error TextBlocks
        private TextBox? _usernameTextBox;
        private TextBox? _passwordTextBox;
        private TextBox? _nameTextBox;
        private TextBox? _lastnameTextBox;
        private TextBox? _ageTextBox;

        private TextBlock? _usernameErrorTextBlock;
        private TextBlock? _passwordErrorTextBlock;
        private TextBlock? _nameErrorTextBlock;
        private TextBlock? _lastnameErrorTextBlock;
        private TextBlock? _ageErrorTextBlock;


        // Login TextBoxes & Error TextBlocks
        private TextBox? _loginUsernameTextBox;
        private TextBox? _loginPasswordTextBox;

        private TextBlock? _loginUsernameErrorTextBlock;
        private TextBlock? _loginPasswordErrorTextBlock;

        public UnauthenticatedAreaView()
        {
            InitializeComponent();

            // Agregar debugging para ver usuarios existentes
            try
            {
                var userCrud = new Project.infrastucture.UserCRUD();
                userCrud.ListAllUsers();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error listing users: {ex.Message}");
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            // Mostrar Register y Login grid
            _registerGrid = this.FindControl<Grid>("RegisterGrid");
            _loginGrid = this.FindControl<Grid>("LoginGrid");

            // TextBox & ErrorTextBlock de validacion de datos del registro
            _usernameTextBox = this.FindControl<TextBox>("UsernameTextBox");
            _passwordTextBox = this.FindControl<TextBox>("PasswordTextBox");
            _nameTextBox = this.FindControl<TextBox>("NameTextBox");
            _lastnameTextBox = this.FindControl<TextBox>("LastnameTextBox");
            _ageTextBox = this.FindControl<TextBox>("AgeTextBox");

            _usernameErrorTextBlock = this.FindControl<TextBlock>("UsernameErrorTextBlock");
            _passwordErrorTextBlock = this.FindControl<TextBlock>("PasswordErrorTextBlock");
            _nameErrorTextBlock = this.FindControl<TextBlock>("NameErrorTextBlock");
            _lastnameErrorTextBlock = this.FindControl<TextBlock>("LastnameErrorTextBlock");
            _ageErrorTextBlock = this.FindControl<TextBlock>("AgeErrorTextBlock");

            // TextBox & ErrorTextBlock de validacion de datos del login
            _loginUsernameTextBox = this.FindControl<TextBox>("LoginUsernameTextBox");
            _loginPasswordTextBox = this.FindControl<TextBox>("LoginPasswordTextBox");

            _loginUsernameErrorTextBlock = this.FindControl<TextBlock>("LoginUsernameErrorTextBlock");
            _loginPasswordErrorTextBlock = this.FindControl<TextBlock>("LoginPasswordErrorTextBlock");


        }

        // Método para dirigirse a LoginGrid
        private void ShowLoginGrid(object? sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ShowLoginGrid called");
            if (_registerGrid == null || _loginGrid == null)
                return;

            _registerGrid.IsVisible = false;
            _loginGrid.IsVisible = true;
        }

        // Método para dirigirse a RegisterGrid
        private void ShowRegisterGrid(object? sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ShowRegisterGrid called");
            if (_registerGrid == null || _loginGrid == null)
                return;

            _registerGrid.IsVisible = true;
            _loginGrid.IsVisible = false;
        }

        private void OnRegisterClick(object? sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== REGISTER BUTTON CLICKED ===");

            var usernameText = _usernameTextBox?.Text;
            var passwordText = _passwordTextBox?.Text;
            var nameText = _nameTextBox?.Text;
            var lastnameText = _lastnameTextBox?.Text;
            var ageText = _ageTextBox?.Text;

            bool validUsername = RegisterAutentification.IsValidUsername(usernameText);
            bool validPassword = RegisterAutentification.IsValidPassword(passwordText);
            bool validName = RegisterAutentification.IsValidName(nameText);
            bool validLastname = RegisterAutentification.IsValidLastname(lastnameText);
            bool validAge = RegisterAutentification.IsValidAge(ageText);

            // Ocultar mensajes de error antes de validar
            _usernameErrorTextBlock!.IsVisible = false;
            _passwordErrorTextBlock!.IsVisible = false;
            _nameErrorTextBlock!.IsVisible = false;
            _lastnameErrorTextBlock!.IsVisible = false;
            _ageErrorTextBlock!.IsVisible = false;

            // Validar los datos ingresados
            if (validUsername && validPassword && validName && validLastname && validAge)
            {
                try
                {
                    // Guardar en la base de datos usando UserCRUD
                    var userCrud = new Project.infrastucture.UserCRUD();

                    if (userCrud.GetUserByUsername(usernameText!) != null)
                    {
                        _usernameErrorTextBlock!.IsVisible = true;
                        _usernameErrorTextBlock.Text = "El nombre de usuario ya está en uso.";
                        return;
                    }


                    var newUser = new Project.domain.models.user
                    {
                        Username = usernameText!,
                        Password = passwordHasher.HashPassword(passwordText!), // ✅ HASHEAR LA CONTRASEÑA
                        Name = nameText!,
                        LastName = lastnameText!,
                        Age = int.Parse(ageText!),
                        PathImage = "" // O asigna un valor por defecto si corresponde
                    };

                    bool registroExitoso = userCrud.SignUp(newUser);

                    if (registroExitoso)
                    {
                        // Registro exitoso, establecer sesión del usuario
                        newUser = userCrud.GetUserByUsername(usernameText!);
                        UserSession.SetCurrentUser(newUser);

                        System.Diagnostics.Debug.WriteLine($"Usuario registrado exitosamente: {usernameText}");

                        // Listar usuarios después del registro
                        userCrud.ListAllUsers();

                        var window = this.VisualRoot as Window;
                        if (window != null)
                        {
                            window.Content = new AuthenticatedAreaView();
                        }
                    }
                    else
                    {
                        // Error en el registro
                        _usernameErrorTextBlock!.IsVisible = true;
                        _usernameErrorTextBlock.Text = "Error al registrar el usuario. Puede que el nombre de usuario ya exista.";
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    System.Diagnostics.Debug.WriteLine($"Error en registro: {ex.Message}");

                    _usernameErrorTextBlock!.IsVisible = true;
                    _usernameErrorTextBlock.Text = "Error interno. Intente nuevamente."; 
                }
            }
            else
            {
                // Mostrar mensajes de error según corresponda
                if (!validUsername)
                {
                    _usernameErrorTextBlock!.Text = "El nombre de usuario no debe contener espacios. Máximo 10 caracteres";
                    _usernameErrorTextBlock.IsVisible = true;
                }
                if (!validPassword)
                {
                    _passwordErrorTextBlock!.Text = "Mínimo 8 caracteres, 1 mayúscula, 1 minúscula y 1 número";
                    _passwordErrorTextBlock.IsVisible = true;
                }
                if (!validName)
                {
                    _nameErrorTextBlock!.Text = "El nombre no puede tener espacios, números o símbolos";
                    _nameErrorTextBlock.IsVisible = true;
                }
                if (!validLastname)
                {
                    _lastnameErrorTextBlock!.Text = "El apellido no puede tener espacios, números o símbolos";
                    _lastnameErrorTextBlock.IsVisible = true;
                }
                if (!validAge)
                {
                    _ageErrorTextBlock!.Text = "La edad debe ser un número entero positivo";
                    _ageErrorTextBlock.IsVisible = true;
                }
            }
        }


        private void OnLoginClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== LOGIN BUTTON CLICKED ===");

            try
            {
                // Busca los TextBox del login
                var loginUsernameTextBox = this.FindControl<TextBox>("LoginUsernameTextBox");
                var loginPasswordTextBox = this.FindControl<TextBox>("LoginPasswordTextBox");

                var username = loginUsernameTextBox?.Text;
                var password = loginPasswordTextBox?.Text;

                System.Diagnostics.Debug.WriteLine($"Username encontrado: {username}");
                System.Diagnostics.Debug.WriteLine($"Password encontrado: {password}");

                // Ocultar mensajes de error previos
                _usernameErrorTextBlock!.IsVisible = false;

                // Validación básica
                if (string.IsNullOrWhiteSpace(username))
                {
                    _loginUsernameErrorTextBlock!.IsVisible = true;
                    _loginUsernameErrorTextBlock.Text = "Por favor, ingrese el nombre de usuario.";
                    return;
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    _loginPasswordErrorTextBlock!.IsVisible = true;
                    _loginPasswordErrorTextBlock.Text = "Por favor, ingrese la contraseña.";
                    return;
                }

                // Agregar debug para diagnóstico
                System.Diagnostics.Debug.WriteLine($"=== INICIANDO LOGIN ===");
                System.Diagnostics.Debug.WriteLine($"Usuario ingresado: '{username}'");
                System.Diagnostics.Debug.WriteLine($"Contraseña ingresada: '{password}'");
                System.Diagnostics.Debug.WriteLine($"Hash de contraseña ingresada: '{passwordHasher.HashPassword(password)}'");

                var userCrud = new Project.infrastucture.UserCRUD();

                // Listar usuarios antes del login para verificar que existen
                System.Diagnostics.Debug.WriteLine("Usuarios en la base de datos antes del login:");
                userCrud.ListAllUsers();

                bool acceso = userCrud.Login(username, password);

                if (acceso)
                {
                    System.Diagnostics.Debug.WriteLine("=== LOGIN EXITOSO ===");
                    var window = this.VisualRoot as Window;
                    if (window != null)
                    {
                        window.Content = new AuthenticatedAreaView();
                    }
                }
                else
                {
                    _loginUsernameErrorTextBlock!.IsVisible = true;
                    _loginUsernameErrorTextBlock.Text = "Nombre de usuario o contraseña incorrectos.";
                }
            }
            catch (Exception)
            {
                _loginUsernameErrorTextBlock!.IsVisible = true;
                _loginUsernameErrorTextBlock.Text = "Error interno. Intente nuevamente.";
            }
        }
    }
}