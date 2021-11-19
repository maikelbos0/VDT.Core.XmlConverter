﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace VDT.Core.Blazor.GlobalEventHandler {
    public class GlobalEventHandler : ComponentBase {
        private IJSObjectReference? moduleReference;
        private DotNetObjectReference<GlobalEventHandler>? dotNetObjectReference;

        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;

        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }
        [Parameter] public EventCallback<ResizeEventArgs> OnResize { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnMouseDown { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnMouseUp { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnMouseMove { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> OnContextMenu { get; set; }        
        [Parameter] public EventCallback<MouseEventArgs> OnDoubleClick { get; set; }

        [JSInvokable] public async Task InvokeKeyDown(KeyboardEventArgs args) => await OnKeyDown.InvokeAsync(args);
        [JSInvokable] public async Task InvokeKeyUp(KeyboardEventArgs args) => await OnKeyUp.InvokeAsync(args);
        [JSInvokable] public async Task InvokeResize(ResizeEventArgs args) => await OnResize.InvokeAsync(args);
        [JSInvokable] public async Task InvokeClick(MouseEventArgs args) => await OnClick.InvokeAsync(args);
        [JSInvokable] public async Task InvokeMouseDown(MouseEventArgs args) => await OnMouseDown.InvokeAsync(args);
        [JSInvokable] public async Task InvokeMouseUp(MouseEventArgs args) => await OnMouseUp.InvokeAsync(args);
        [JSInvokable] public async Task InvokeMouseMove(MouseEventArgs args) => await OnMouseMove.InvokeAsync(args);
        [JSInvokable] public async Task InvokeContextMenu(MouseEventArgs args) => await OnContextMenu.InvokeAsync(args);
        [JSInvokable] public async Task InvokeDoubleClick(MouseEventArgs args) => await OnDoubleClick.InvokeAsync(args);

        protected override bool ShouldRender() => false;

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                moduleReference = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/VDT.Core.Blazor.GlobalEventHandler/globaleventhandler.js");
                dotNetObjectReference = DotNetObjectReference.Create(this);

                await moduleReference.InvokeVoidAsync("register", dotNetObjectReference);
            }
        }

        public async ValueTask DisposeAsync() {
            if (moduleReference != null) {
                await moduleReference.InvokeVoidAsync("unregister", dotNetObjectReference);
                await moduleReference.DisposeAsync();
            }

            dotNetObjectReference?.Dispose();
        }
    }
}
