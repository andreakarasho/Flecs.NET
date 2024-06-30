using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Flecs.NET.Collections;
using Flecs.NET.Utilities;
using static Flecs.NET.Bindings.flecs;

namespace Flecs.NET.Core
{
    /// <summary>
    ///     A static class holding methods and types for binding contexts.
    /// </summary>
    public static unsafe class BindingContext
    {
        static BindingContext()
        {
            AppDomain.CurrentDomain.ProcessExit += (object? sender, EventArgs e) =>
            {
                Memory.Free(DefaultSeparator);
            };
        }

        internal static readonly byte* DefaultSeparator = (byte*)Marshal.StringToHGlobalAnsi(Ecs.DefaultSeparator);

#if NET5_0_OR_GREATER
        internal static readonly IntPtr ActionCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&ActionCallback;
        internal static readonly IntPtr IterCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&IterCallback;
        internal static readonly IntPtr EachEntityCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&EachEntityCallback;
        internal static readonly IntPtr EachIterCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&EachIterCallback;
        internal static readonly IntPtr ObserveCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&ObserveCallback;
        internal static readonly IntPtr RunCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&RunCallback;
        internal static readonly IntPtr RunDelegateCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&RunDelegateCallback;
        internal static readonly IntPtr RunPointerCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&RunPointerCallback;
        internal static readonly IntPtr GroupByCallbackPointer = (IntPtr)(delegate* <ecs_world_t*, ecs_table_t*, ulong, void*, ulong>)&GroupByCallback;

        internal static readonly IntPtr WorldContextFreePointer = (IntPtr)(delegate* <void*, void>)&WorldContextFree;
        internal static readonly IntPtr IteratorContextFreePointer = (IntPtr)(delegate* <void*, void>)&IteratorContextFree;
        internal static readonly IntPtr RunContextFreePointer = (IntPtr)(delegate* <void*, void>)&RunContextFree;
        internal static readonly IntPtr QueryContextFreePointer = (IntPtr)(delegate* <void*, void>)&QueryContextFree;
        internal static readonly IntPtr GroupByContextFreePointer = (IntPtr)(delegate* <void*, void>)&GroupByContextFree;
        internal static readonly IntPtr TypeHooksContextFreePointer = (IntPtr)(delegate* <void*, void>)&TypeHooksContextFree;

        internal static readonly IntPtr OsApiAbortPointer = (IntPtr)(delegate* <void>)&OsApiAbort;
#else
        private static readonly Ecs.IterAction ActionCallbackReference = ActionCallback;
        private static readonly Ecs.IterAction IterCallbackReference = IterCallback;
        private static readonly Ecs.IterAction EachEntityCallbackReference = EachEntityCallback;
        private static readonly Ecs.IterAction EachIterCallbackReference = EachIterCallback;
        private static readonly Ecs.IterAction ObserveCallbackReference = ObserveCallback;
        private static readonly Ecs.IterAction RunCallbackReference = RunCallback;
        private static readonly Ecs.IterAction RunDelegateCallbackReference = RunDelegateCallback;
        private static readonly Ecs.GroupByAction GroupByCallbackReference = GroupByCallback;

        private static readonly Ecs.ContextFree WorldContextFreeReference = WorldContextFree;
        private static readonly Ecs.ContextFree IteratorContextFreeReference = IteratorContextFree;
        private static readonly Ecs.ContextFree RunContextFreeReference = RunContextFree;
        private static readonly Ecs.ContextFree QueryContextFreeReference = QueryContextFree;
        private static readonly Ecs.ContextFree GroupByContextFreeReference = GroupByContextFree;
        private static readonly Ecs.ContextFree TypeHooksContextFreeReference = TypeHooksContextFree;

        private static readonly Action OsApiAbortReference = OsApiAbort;

        internal static readonly IntPtr ActionCallbackPointer = Marshal.GetFunctionPointerForDelegate(ActionCallbackReference);
        internal static readonly IntPtr IterCallbackPointer = Marshal.GetFunctionPointerForDelegate(IterCallbackReference);
        internal static readonly IntPtr EachEntityCallbackPointer = Marshal.GetFunctionPointerForDelegate(EachEntityCallbackReference);
        internal static readonly IntPtr EachIterCallbackPointer = Marshal.GetFunctionPointerForDelegate(EachIterCallbackReference);
        internal static readonly IntPtr ObserveCallbackPointer = Marshal.GetFunctionPointerForDelegate(ObserveCallbackReference);
        internal static readonly IntPtr RunCallbackPointer = Marshal.GetFunctionPointerForDelegate(RunCallbackReference);
        internal static readonly IntPtr RunDelegateCallbackPointer = Marshal.GetFunctionPointerForDelegate(RunDelegateCallbackReference);
        internal static readonly IntPtr GroupByCallbackPointer = Marshal.GetFunctionPointerForDelegate(GroupByCallbackReference);

        internal static readonly IntPtr WorldContextFreePointer = Marshal.GetFunctionPointerForDelegate(WorldContextFreeReference);
        internal static readonly IntPtr IteratorContextFreePointer = Marshal.GetFunctionPointerForDelegate(IteratorContextFreeReference);
        internal static readonly IntPtr RunContextFreePointer = Marshal.GetFunctionPointerForDelegate(RunContextFreeReference);
        internal static readonly IntPtr QueryContextFreePointer = Marshal.GetFunctionPointerForDelegate(QueryContextFreeReference);
        internal static readonly IntPtr GroupByContextFreePointer = Marshal.GetFunctionPointerForDelegate(GroupByContextFreeReference);
        internal static readonly IntPtr TypeHooksContextFreePointer = Marshal.GetFunctionPointerForDelegate(TypeHooksContextFreeReference);

        internal static readonly IntPtr OsApiAbortPointer = Marshal.GetFunctionPointerForDelegate(OsApiAbortReference);
#endif

        private static void ActionCallback(ecs_iter_t* iter)
        {
            IteratorContext* context = (IteratorContext*)iter->callback_ctx;

#if NET5_0_OR_GREATER
            if (context->Callback.GcHandle == default)
            {
                ((delegate*<void>)context->Callback.Function)();
                return;
            }
#endif

            Action callback = (Action)context->Callback.GcHandle.Target!;
            callback();
        }

        private static void IterCallback(ecs_iter_t* iter)
        {
            IteratorContext* context = (IteratorContext*)iter->callback_ctx;

#if NET5_0_OR_GREATER
            if (context->Callback.GcHandle == default)
            {
                Invoker.Iter(iter, (delegate*<Iter, void>)context->Callback.Function);
                return;
            }
#endif

            Ecs.IterCallback callback = (Ecs.IterCallback)context->Callback.GcHandle.Target!;
            Invoker.Iter(iter, callback);
        }

        private static void EachEntityCallback(ecs_iter_t* iter)
        {
            IteratorContext* context = (IteratorContext*)iter->callback_ctx;

#if NET5_0_OR_GREATER
            if (context->Callback.GcHandle == default)
            {
                Invoker.Each(iter, (delegate*<Entity, void>)context->Callback.Function);
                return;
            }
#endif

            Ecs.EachEntityCallback callback = (Ecs.EachEntityCallback)context->Callback.GcHandle.Target!;
            Invoker.Each(iter, callback);
        }

        private static void EachIterCallback(ecs_iter_t* iter)
        {
            IteratorContext* context = (IteratorContext*)iter->callback_ctx;

#if NET5_0_OR_GREATER
            if (context->Callback.GcHandle == default)
            {
                Invoker.Each(iter, (delegate*<Iter, int, void>)context->Callback.Function);
                return;
            }
#endif

            Ecs.EachIterCallback callback = (Ecs.EachIterCallback)context->Callback.GcHandle.Target!;
            Invoker.Each(iter, callback);
        }

        private static void ObserveCallback(ecs_iter_t* iter)
        {
            IteratorContext* context = (IteratorContext*)iter->callback_ctx;

#if NET5_0_OR_GREATER
            if (context->Callback.GcHandle == default)
            {
                Invoker.Each(iter, (delegate*<Entity, void>)context->Callback.Function);
                return;
            }
#endif

            Ecs.EachEntityCallback callback = (Ecs.EachEntityCallback)context->Callback.GcHandle.Target!;
            Invoker.Observe(iter, callback);
        }

        private static void RunCallback(ecs_iter_t* iter)
        {
            RunContext* context = (RunContext*)iter->run_ctx;

#if NET5_0_OR_GREATER
            if (context->Callback.GcHandle == default)
            {
                Invoker.Run(iter, (delegate*<Iter, void>)context->Callback.Function);
                return;
            }
#endif

            Ecs.RunCallback callback = (Ecs.RunCallback)context->Callback.GcHandle.Target!;
            Invoker.Run(iter, callback);
        }

        private static void RunDelegateCallback(ecs_iter_t* iter)
        {
            RunContext* context = (RunContext*)iter->run_ctx;

#if NET5_0_OR_GREATER
            if (context->Callback.GcHandle == default)
            {
                Invoker.Run(iter, (delegate*<Iter, Action<Iter>, void>)context->Callback.Function);
                return;
            }
#endif

            Ecs.RunDelegateCallback callback = (Ecs.RunDelegateCallback)context->Callback.GcHandle.Target!;
            Invoker.Run(iter, callback);
        }

#if NET5_0_OR_GREATER
        private static void RunPointerCallback(ecs_iter_t* iter)
        {
            RunContext* context = (RunContext*)iter->run_ctx;

            if (context->Callback.GcHandle == default)
            {
                Invoker.Run(iter, (delegate*<Iter, delegate*<Iter, void>, void>)context->Callback.Function);
                return;
            }

            Ecs.RunPointerCallback callback = (Ecs.RunPointerCallback)context->Callback.GcHandle.Target!;
            Invoker.Run(iter, callback);
        }
#endif

        private static ulong GroupByCallback(ecs_world_t* world, ecs_table_t* table, ulong id, void* ctx)
        {
            GroupByContext* context = (GroupByContext*)ctx;
            Ecs.GroupByCallback callback = (Ecs.GroupByCallback)context->GroupBy.GcHandle.Target!;
            return callback(new World(world), new Table(world, table), new Entity(world, id));
        }

        private static void WorldContextFree(void* context)
        {
            WorldContext* worldContext = (WorldContext*)context;
            worldContext->Dispose();
            Memory.Free(context);
        }

        private static void IteratorContextFree(void* context)
        {
            IteratorContext* iteratorContext = (IteratorContext*)context;
            iteratorContext->Dispose();
            Memory.Free(context);
        }

        private static void RunContextFree(void* context)
        {
            RunContext* runContext = (RunContext*)context;
            runContext->Dispose();
            Memory.Free(context);
        }

        private static void QueryContextFree(void* context)
        {
            QueryContext* queryContext = (QueryContext*)context;
            queryContext->Dispose();
            Memory.Free(context);
        }

        private static void GroupByContextFree(void* context)
        {
            GroupByContext* queryGroupByContext = (GroupByContext*)context;
            queryGroupByContext->Dispose();
            Memory.Free(context);
        }

        private static void TypeHooksContextFree(void* context)
        {
            TypeHooksContext* typeHooks = (TypeHooksContext*)context;
            typeHooks->Dispose();
            Memory.Free(context);
        }

        private static void OsApiAbort()
        {
            throw new Ecs.NativeException("Application aborted from native code.");
        }

        internal static Callback AllocCallback<T>(T? callback, bool storePtr = true) where T : Delegate
        {
            if (callback == null)
                return default;

            IntPtr funcPtr = storePtr ? Marshal.GetFunctionPointerForDelegate(callback) : IntPtr.Zero;
            return new Callback(funcPtr, GCHandle.Alloc(callback));
        }

        internal static void SetCallback(ref Callback dest, IntPtr callback)
        {
            if (dest.GcHandle != default)
                dest.Dispose();

            dest.Function = callback;
        }

        internal static void SetCallback<T>(ref Callback dest, T? callback, bool storePtr = true) where T : Delegate
        {
            if (dest.GcHandle != default)
                dest.Dispose();

            dest = AllocCallback(callback, storePtr);
        }

        internal struct Callback : IDisposable
        {
            public IntPtr Function;
            public GCHandle GcHandle;

            public Callback(IntPtr function, GCHandle gcHandle)
            {
                Function = function;
                GcHandle = gcHandle;
            }

            public void Dispose()
            {
                Managed.FreeGcHandle(GcHandle);
                Function = default;
                GcHandle = default;
            }
        }

        internal class Box<T>
        {
            public bool ShouldFree;
            [MaybeNull] public T Value = default!;

            public Box()
            {
            }

            public Box(T value, bool shouldFree = false)
            {
                Value = value;
                ShouldFree = shouldFree;
            }
        }

        internal struct WorldContext : IDisposable
        {
            public Callback AtFini;
            public Callback RunPostFrame;
            public Callback ContextFree;
            public NativeList<ulong> TypeCache;

            public void Dispose()
            {
                AtFini.Dispose();
                RunPostFrame.Dispose();
                ContextFree.Dispose();
                TypeCache.Dispose();
            }
        }

        internal struct IteratorContext : IDisposable
        {
            public Callback Callback;

            public void Dispose()
            {
                Callback.Dispose();
            }
        }

        internal struct RunContext : IDisposable
        {
            public Callback Callback;

            public void Dispose()
            {
                Callback.Dispose();
            }
        }

        internal struct QueryContext : IDisposable
        {
            public Callback OrderByAction;
            public Callback GroupByAction;
            public Callback ContextFree;
            public Callback GroupCreateAction;
            public Callback GroupDeleteAction;

            public NativeList<NativeString> Strings;

            public void Dispose()
            {
                OrderByAction.Dispose();
                GroupByAction.Dispose();
                ContextFree.Dispose();
                GroupCreateAction.Dispose();
                GroupDeleteAction.Dispose();

                if (Strings == default)
                    return;

                for (int i = 0; i < Strings.Count; i++)
                    Strings[i].Dispose();

                Strings.Dispose();
            }
        }

        internal struct TypeHooksContext : IDisposable
        {
            public int Header;

            public Callback Ctor;
            public Callback Dtor;
            public Callback Move;
            public Callback Copy;
            public Callback OnAdd;
            public Callback OnSet;
            public Callback OnRemove;
            public Callback ContextFree;

            public static readonly TypeHooksContext Default = new TypeHooksContext { Header = Ecs.Header };

            public void Dispose()
            {
                Ctor.Dispose();
                Dtor.Dispose();
                Move.Dispose();
                Copy.Dispose();
                OnAdd.Dispose();
                OnSet.Dispose();
                OnRemove.Dispose();
                ContextFree.Dispose();
            }
        }

        internal struct GroupByContext : IDisposable
        {
            public Callback GroupBy;

            public void Dispose()
            {
                GroupBy.Dispose();
            }
        }
    }

    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    internal static unsafe partial class BindingContext<T0>
    {
#if NET5_0_OR_GREATER
        internal static readonly IntPtr EntityObserverEachPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&EntityObserverEach;
        internal static readonly IntPtr EntityObserverEachEntityPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&EntityObserverEachEntity;

        internal static readonly IntPtr UnmanagedCtorPointer = (IntPtr)(delegate* <void*, int, ecs_type_info_t*, void>)&UnmanagedCtor;
        internal static readonly IntPtr UnmanagedDtorPointer = (IntPtr)(delegate* <void*, int, ecs_type_info_t*, void>)&UnmanagedDtor;
        internal static readonly IntPtr UnmanagedMovePointer = (IntPtr)(delegate* <void*, void*, int, ecs_type_info_t*, void>)&UnmanagedMove;
        internal static readonly IntPtr UnmanagedCopyPointer = (IntPtr)(delegate* <void*, void*, int, ecs_type_info_t*, void>)&UnmanagedCopy;

        internal static readonly IntPtr ManagedCtorPointer = (IntPtr)(delegate* <void*, int, ecs_type_info_t*, void>)&ManagedCtor;
        internal static readonly IntPtr ManagedDtorPointer = (IntPtr)(delegate* <void*, int, ecs_type_info_t*, void>)&ManagedDtor;
        internal static readonly IntPtr ManagedMovePointer = (IntPtr)(delegate* <void*, void*, int, ecs_type_info_t*, void>)&ManagedMove;
        internal static readonly IntPtr ManagedCopyPointer = (IntPtr)(delegate* <void*, void*, int, ecs_type_info_t*, void>)&ManagedCopy;

        internal static readonly IntPtr DefaultManagedCtorPointer = (IntPtr)(delegate* <void*, int, ecs_type_info_t*, void>)&DefaultManagedCtor;
        internal static readonly IntPtr DefaultManagedDtorPointer = (IntPtr)(delegate* <void*, int, ecs_type_info_t*, void>)&DefaultManagedDtor;
        internal static readonly IntPtr DefaultManagedMovePointer = (IntPtr)(delegate* <void*, void*, int, ecs_type_info_t*, void>)&DefaultManagedMove;
        internal static readonly IntPtr DefaultManagedCopyPointer = (IntPtr)(delegate* <void*, void*, int, ecs_type_info_t*, void>)&DefaultManagedCopy;

        internal static readonly IntPtr OnAddIterFieldCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnAddIterFieldCallback;
        internal static readonly IntPtr OnSetIterFieldCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnSetIterFieldCallback;
        internal static readonly IntPtr OnRemoveIterFieldCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnRemoveIterFieldCallback;

        internal static readonly IntPtr OnAddIterSpanCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnAddIterSpanCallback;
        internal static readonly IntPtr OnSetIterSpanCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnSetIterSpanCallback;
        internal static readonly IntPtr OnRemoveIterSpanCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnRemoveIterSpanCallback;

        internal static readonly IntPtr OnAddIterPointerCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnAddIterPointerCallback;
        internal static readonly IntPtr OnSetIterPointerCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnSetIterPointerCallback;
        internal static readonly IntPtr OnRemoveIterPointerCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnRemoveIterPointerCallback;

        internal static readonly IntPtr OnAddEachRefCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnAddEachRefCallback;
        internal static readonly IntPtr OnSetEachRefCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnSetEachRefCallback;
        internal static readonly IntPtr OnRemoveEachRefCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnRemoveEachRefCallback;

        internal static readonly IntPtr OnAddEachEntityRefCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnAddEachEntityRefCallback;
        internal static readonly IntPtr OnSetEachEntityRefCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnSetEachEntityRefCallback;
        internal static readonly IntPtr OnRemoveEachEntityRefCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnRemoveEachEntityRefCallback;

        internal static readonly IntPtr OnAddEachIterRefCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnAddEachIterRefCallback;
        internal static readonly IntPtr OnSetEachIterRefCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnSetEachIterRefCallback;
        internal static readonly IntPtr OnRemoveEachIterRefCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnRemoveEachIterRefCallback;

        internal static readonly IntPtr OnAddEachPointerCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnAddEachPointerCallback;
        internal static readonly IntPtr OnSetEachPointerCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnSetEachPointerCallback;
        internal static readonly IntPtr OnRemoveEachPointerCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnRemoveEachPointerCallback;

        internal static readonly IntPtr OnAddEachEntityPointerCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnAddEachEntityPointerCallback;
        internal static readonly IntPtr OnSetEachEntityPointerCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnSetEachEntityPointerCallback;
        internal static readonly IntPtr OnRemoveEachEntityPointerCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnRemoveEachEntityPointerCallback;

        internal static readonly IntPtr OnAddEachIterPointerCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnAddEachIterPointerCallback;
        internal static readonly IntPtr OnSetEachIterPointerCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnSetEachIterPointerCallback;
        internal static readonly IntPtr OnRemoveEachIterPointerCallbackPointer = (IntPtr)(delegate* <ecs_iter_t*, void>)&OnRemoveEachIterPointerCallback;

#else
        private static readonly Ecs.IterAction EntityObserverEachReference = EntityObserverEach;
        private static readonly Ecs.IterAction EntityObserverEachEntityReference = EntityObserverEachEntity;

        private static readonly Ecs.CtorCallback UnmanagedCtorReference = UnmanagedCtor;
        private static readonly Ecs.DtorCallback UnmanagedDtorReference = UnmanagedDtor;
        private static readonly Ecs.MoveCallback UnmanagedMoveReference = UnmanagedMove;
        private static readonly Ecs.CopyCallback UnmanagedCopyReference = UnmanagedCopy;

        private static readonly Ecs.CtorCallback ManagedCtorReference = ManagedCtor;
        private static readonly Ecs.DtorCallback ManagedDtorReference = ManagedDtor;
        private static readonly Ecs.MoveCallback ManagedMoveReference = ManagedMove;
        private static readonly Ecs.CopyCallback ManagedCopyReference = ManagedCopy;

        private static readonly Ecs.CtorCallback DefaultManagedCtorReference = DefaultManagedCtor;
        private static readonly Ecs.DtorCallback DefaultManagedDtorReference = DefaultManagedDtor;
        private static readonly Ecs.MoveCallback DefaultManagedMoveReference = DefaultManagedMove;
        private static readonly Ecs.CopyCallback DefaultManagedCopyReference = DefaultManagedCopy;

        private static readonly Ecs.IterAction OnAddIterFieldCallbackReference = OnAddIterFieldCallback;
        private static readonly Ecs.IterAction OnSetIterFieldCallbackReference = OnSetIterFieldCallback;
        private static readonly Ecs.IterAction OnRemoveIterFieldCallbackReference = OnRemoveIterFieldCallback;

        private static readonly Ecs.IterAction OnAddIterSpanCallbackReference = OnAddIterSpanCallback;
        private static readonly Ecs.IterAction OnSetIterSpanCallbackReference = OnSetIterSpanCallback;
        private static readonly Ecs.IterAction OnRemoveIterSpanCallbackReference = OnRemoveIterSpanCallback;

        private static readonly Ecs.IterAction OnAddIterPointerCallbackReference = OnAddIterPointerCallback;
        private static readonly Ecs.IterAction OnSetIterPointerCallbackReference = OnSetIterPointerCallback;
        private static readonly Ecs.IterAction OnRemoveIterPointerCallbackReference = OnRemoveIterPointerCallback;

        private static readonly Ecs.IterAction OnAddEachRefCallbackReference = OnAddEachRefCallback;
        private static readonly Ecs.IterAction OnSetEachRefCallbackReference = OnSetEachRefCallback;
        private static readonly Ecs.IterAction OnRemoveEachRefCallbackReference = OnRemoveEachRefCallback;

        private static readonly Ecs.IterAction OnAddEachEntityRefCallbackReference = OnAddEachEntityRefCallback;
        private static readonly Ecs.IterAction OnSetEachEntityRefCallbackReference = OnSetEachEntityRefCallback;
        private static readonly Ecs.IterAction OnRemoveEachEntityRefCallbackReference = OnRemoveEachEntityRefCallback;

        private static readonly Ecs.IterAction OnAddEachIterRefCallbackReference = OnAddEachIterRefCallback;
        private static readonly Ecs.IterAction OnSetEachIterRefCallbackReference = OnSetEachIterRefCallback;
        private static readonly Ecs.IterAction OnRemoveEachIterRefCallbackReference = OnRemoveEachIterRefCallback;

        private static readonly Ecs.IterAction OnAddEachPointerCallbackReference = OnAddEachPointerCallback;
        private static readonly Ecs.IterAction OnSetEachPointerCallbackReference = OnSetEachPointerCallback;
        private static readonly Ecs.IterAction OnRemoveEachPointerCallbackReference = OnRemoveEachPointerCallback;

        private static readonly Ecs.IterAction OnAddEachEntityPointerCallbackReference = OnAddEachEntityPointerCallback;
        private static readonly Ecs.IterAction OnSetEachEntityPointerCallbackReference = OnSetEachEntityPointerCallback;
        private static readonly Ecs.IterAction OnRemoveEachEntityPointerCallbackReference = OnRemoveEachEntityPointerCallback;

        private static readonly Ecs.IterAction OnAddEachIterPointerCallbackReference = OnAddEachIterPointerCallback;
        private static readonly Ecs.IterAction OnSetEachIterPointerCallbackReference = OnSetEachIterPointerCallback;
        private static readonly Ecs.IterAction OnRemoveEachIterPointerCallbackReference = OnRemoveEachIterPointerCallback;

        internal static readonly IntPtr EntityObserverEachPointer = Marshal.GetFunctionPointerForDelegate(EntityObserverEachReference);
        internal static readonly IntPtr EntityObserverEachEntityPointer = Marshal.GetFunctionPointerForDelegate(EntityObserverEachEntityReference);

        internal static readonly IntPtr UnmanagedCtorPointer = Marshal.GetFunctionPointerForDelegate(UnmanagedCtorReference);
        internal static readonly IntPtr UnmanagedDtorPointer = Marshal.GetFunctionPointerForDelegate(UnmanagedDtorReference);
        internal static readonly IntPtr UnmanagedMovePointer = Marshal.GetFunctionPointerForDelegate(UnmanagedMoveReference);
        internal static readonly IntPtr UnmanagedCopyPointer = Marshal.GetFunctionPointerForDelegate(UnmanagedCopyReference);

        internal static readonly IntPtr ManagedCtorPointer = Marshal.GetFunctionPointerForDelegate(ManagedCtorReference);
        internal static readonly IntPtr ManagedDtorPointer = Marshal.GetFunctionPointerForDelegate(ManagedDtorReference);
        internal static readonly IntPtr ManagedMovePointer = Marshal.GetFunctionPointerForDelegate(ManagedMoveReference);
        internal static readonly IntPtr ManagedCopyPointer = Marshal.GetFunctionPointerForDelegate(ManagedCopyReference);

        internal static readonly IntPtr DefaultManagedCtorPointer = Marshal.GetFunctionPointerForDelegate(DefaultManagedCtorReference);
        internal static readonly IntPtr DefaultManagedDtorPointer = Marshal.GetFunctionPointerForDelegate(DefaultManagedDtorReference);
        internal static readonly IntPtr DefaultManagedMovePointer = Marshal.GetFunctionPointerForDelegate(DefaultManagedMoveReference);
        internal static readonly IntPtr DefaultManagedCopyPointer = Marshal.GetFunctionPointerForDelegate(DefaultManagedCopyReference);

        internal static readonly IntPtr OnAddIterFieldCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnAddIterFieldCallbackReference);
        internal static readonly IntPtr OnSetIterFieldCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnSetIterFieldCallbackReference);
        internal static readonly IntPtr OnRemoveIterFieldCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnRemoveIterFieldCallbackReference);

        internal static readonly IntPtr OnAddIterSpanCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnAddIterSpanCallbackReference);
        internal static readonly IntPtr OnSetIterSpanCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnSetIterSpanCallbackReference);
        internal static readonly IntPtr OnRemoveIterSpanCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnRemoveIterSpanCallbackReference);

        internal static readonly IntPtr OnAddIterPointerCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnAddIterPointerCallbackReference);
        internal static readonly IntPtr OnSetIterPointerCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnSetIterPointerCallbackReference);
        internal static readonly IntPtr OnRemoveIterPointerCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnRemoveIterPointerCallbackReference);

        internal static readonly IntPtr OnAddEachRefCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnAddEachRefCallbackReference);
        internal static readonly IntPtr OnSetEachRefCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnSetEachRefCallbackReference);
        internal static readonly IntPtr OnRemoveEachRefCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnRemoveEachRefCallbackReference);

        internal static readonly IntPtr OnAddEachEntityRefCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnAddEachEntityRefCallbackReference);
        internal static readonly IntPtr OnSetEachEntityRefCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnSetEachEntityRefCallbackReference);
        internal static readonly IntPtr OnRemoveEachEntityRefCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnRemoveEachEntityRefCallbackReference);

        internal static readonly IntPtr OnAddEachIterRefCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnAddEachIterRefCallbackReference);
        internal static readonly IntPtr OnSetEachIterRefCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnSetEachIterRefCallbackReference);
        internal static readonly IntPtr OnRemoveEachIterRefCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnRemoveEachIterRefCallbackReference);

        internal static readonly IntPtr OnAddEachPointerCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnAddEachPointerCallbackReference);
        internal static readonly IntPtr OnSetEachPointerCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnSetEachPointerCallbackReference);
        internal static readonly IntPtr OnRemoveEachPointerCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnRemoveEachPointerCallbackReference);

        internal static readonly IntPtr OnAddEachEntityPointerCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnAddEachEntityPointerCallbackReference);
        internal static readonly IntPtr OnSetEachEntityPointerCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnSetEachEntityPointerCallbackReference);
        internal static readonly IntPtr OnRemoveEachEntityPointerCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnRemoveEachEntityPointerCallbackReference);

        internal static readonly IntPtr OnAddEachIterPointerCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnAddEachIterPointerCallbackReference);
        internal static readonly IntPtr OnSetEachIterPointerCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnSetEachIterPointerCallbackReference);
        internal static readonly IntPtr OnRemoveEachIterPointerCallbackPointer = Marshal.GetFunctionPointerForDelegate(OnRemoveEachIterPointerCallbackReference);
#endif

        private static void EntityObserverEach(ecs_iter_t* iter)
        {
            BindingContext.IteratorContext* context = (BindingContext.IteratorContext*)iter->callback_ctx;
            Ecs.EachRefCallback<T0> callback = (Ecs.EachRefCallback<T0>)context->Callback.GcHandle.Target!;
            Invoker.Observe(iter, callback);
        }

        private static void EntityObserverEachEntity(ecs_iter_t* iter)
        {
            BindingContext.IteratorContext* context = (BindingContext.IteratorContext*)iter->callback_ctx;
            Ecs.EachEntityRefCallback<T0> callback = (Ecs.EachEntityRefCallback<T0>)context->Callback.GcHandle.Target!;
            Invoker.Observe(iter, callback);
        }

        private static void UnmanagedCtor(void* dataHandle, int count, ecs_type_info_t* typeInfoHandle)
        {
            TypeInfo typeInfo = new TypeInfo(typeInfoHandle);

            BindingContext.TypeHooksContext* context =
                (BindingContext.TypeHooksContext*)typeInfoHandle->hooks.binding_ctx;

            Ecs.CtorCallback<T0> callback = (Ecs.CtorCallback<T0>)context->Ctor.GcHandle.Target!;

            T0* data = (T0*)dataHandle;

            for (int i = 0; i < count; i++)
                callback(ref data[i], typeInfo);
        }

        private static void UnmanagedDtor(void* dataHandle, int count, ecs_type_info_t* typeInfoHandle)
        {
            TypeInfo typeInfo = new TypeInfo(typeInfoHandle);

            BindingContext.TypeHooksContext* context =
                (BindingContext.TypeHooksContext*)typeInfoHandle->hooks.binding_ctx;

            Ecs.DtorCallback<T0> callback = (Ecs.DtorCallback<T0>)context->Dtor.GcHandle.Target!;

            T0* data = (T0*)dataHandle;

            for (int i = 0; i < count; i++)
                callback(ref data[i], typeInfo);
        }

        private static void UnmanagedMove(void* dstHandle, void* srcHandle, int count, ecs_type_info_t* typeInfoHandle)
        {
            TypeInfo typeInfo = new TypeInfo(typeInfoHandle);

            BindingContext.TypeHooksContext* context =
                (BindingContext.TypeHooksContext*)typeInfoHandle->hooks.binding_ctx;

            Ecs.MoveCallback<T0> callback = (Ecs.MoveCallback<T0>)context->Move.GcHandle.Target!;

            T0* dst = (T0*)dstHandle;
            T0* src = (T0*)srcHandle;

            for (int i = 0; i < count; i++)
                callback(ref dst[i], ref src[i], typeInfo);
        }

        private static void UnmanagedCopy(void* dstHandle, void* srcHandle, int count, ecs_type_info_t* typeInfoHandle)
        {
            TypeInfo typeInfo = new TypeInfo(typeInfoHandle);

            BindingContext.TypeHooksContext* context =
                (BindingContext.TypeHooksContext*)typeInfoHandle->hooks.binding_ctx;

            Ecs.CopyCallback<T0> callback = (Ecs.CopyCallback<T0>)context->Copy.GcHandle.Target!;

            T0* dst = (T0*)dstHandle;
            T0* src = (T0*)srcHandle;

            for (int i = 0; i < count; i++)
                callback(ref dst[i], ref src[i], typeInfo);
        }

        private static void ManagedCtor(void* data, int count, ecs_type_info_t* typeInfoHandle)
        {
            BindingContext.TypeHooksContext* context =
                (BindingContext.TypeHooksContext*)typeInfoHandle->hooks.binding_ctx;
            Ecs.CtorCallback<T0> callback = (Ecs.CtorCallback<T0>)context->Ctor.GcHandle.Target!;

            GCHandle* handles = (GCHandle*)data;

            for (int i = 0; i < count; i++)
            {
                BindingContext.Box<T0> box = new BindingContext.Box<T0>();

                callback(ref box.Value!, new TypeInfo(typeInfoHandle));

                handles[i] = GCHandle.Alloc(box);
            }
        }

        private static void ManagedDtor(void* data, int count, ecs_type_info_t* typeInfoHandle)
        {
            BindingContext.TypeHooksContext* context =
                (BindingContext.TypeHooksContext*)typeInfoHandle->hooks.binding_ctx;
            Ecs.DtorCallback<T0> callback = (Ecs.DtorCallback<T0>)context->Dtor.GcHandle.Target!;

            GCHandle* handles = (GCHandle*)data;

            for (int i = 0; i < count; i++)
            {
                BindingContext.Box<T0> box = (BindingContext.Box<T0>)handles[i].Target!;

                callback(ref box.Value!, new TypeInfo(typeInfoHandle));

                Managed.FreeGcHandle(handles[i]);
                handles[i] = default;
            }
        }

        private static void ManagedMove(void* dst, void* src, int count, ecs_type_info_t* typeInfoHandle)
        {
            BindingContext.TypeHooksContext* context =
                (BindingContext.TypeHooksContext*)typeInfoHandle->hooks.binding_ctx;
            Ecs.MoveCallback<T0> callback = (Ecs.MoveCallback<T0>)context->Move.GcHandle.Target!;

            GCHandle* dstHandles = (GCHandle*)dst;
            GCHandle* srcHandles = (GCHandle*)src;

            for (int i = 0; i < count; i++)
            {
                BindingContext.Box<T0> dstBox = (BindingContext.Box<T0>)dstHandles[i].Target!;
                BindingContext.Box<T0> srcBox = (BindingContext.Box<T0>)srcHandles[i].Target!;

                callback(ref dstBox.Value!, ref srcBox.Value!, new TypeInfo(typeInfoHandle));

                // Free the gc handle if it comes from a .Set call, otherwise let the Dtor hook handle it.
                if (srcBox.ShouldFree)
                    Managed.FreeGcHandle(srcHandles[i]);
            }
        }

        private static void ManagedCopy(void* dst, void* src, int count, ecs_type_info_t* typeInfoHandle)
        {
            BindingContext.TypeHooksContext* context =
                (BindingContext.TypeHooksContext*)typeInfoHandle->hooks.binding_ctx;
            Ecs.CopyCallback<T0> callback = (Ecs.CopyCallback<T0>)context->Copy.GcHandle.Target!;

            GCHandle* dstHandles = (GCHandle*)dst;
            GCHandle* srcHandles = (GCHandle*)src;

            for (int i = 0; i < count; i++)
            {
                BindingContext.Box<T0> dstBox = (BindingContext.Box<T0>)dstHandles[i].Target!;
                BindingContext.Box<T0> srcBox = (BindingContext.Box<T0>)srcHandles[i].Target!;

                callback(ref dstBox.Value!, ref srcBox.Value!, new TypeInfo(typeInfoHandle));

                // Free the gc handle if it comes from a .Set call, otherwise let the Dtor hook handle it.
                if (srcBox.ShouldFree)
                    Managed.FreeGcHandle(srcHandles[i]);
            }
        }

        // For managed types, create a strong box and attempt to set it's value with Activator.CreateInstance<T0>().
        // If no public parameterless constructor exists, the strong box will point to a null value until
        // .Set is called.
        private static void DefaultManagedCtor(void* data, int count, ecs_type_info_t* typeInfoHandle)
        {
            GCHandle* handles = (GCHandle*)data;

            for (int i = 0; i < count; i++)
                handles[i] = GCHandle.Alloc(new BindingContext.Box<T0>());
        }

        private static void DefaultManagedDtor(void* data, int count, ecs_type_info_t* typeInfoHandle)
        {
            GCHandle* handles = (GCHandle*)data;

            for (int i = 0; i < count; i++)
            {
                Managed.FreeGcHandle(handles[i]);
                handles[i] = default;
            }
        }

        private static void DefaultManagedMove(void* dst, void* src, int count, ecs_type_info_t* typeInfoHandle)
        {
            GCHandle* dstHandles = (GCHandle*)dst;
            GCHandle* srcHandles = (GCHandle*)src;

            for (int i = 0; i < count; i++)
            {
                BindingContext.Box<T0> dstBox = (BindingContext.Box<T0>)dstHandles[i].Target!;
                BindingContext.Box<T0> srcBox = (BindingContext.Box<T0>)srcHandles[i].Target!;

                dstBox.Value = srcBox.Value!;

                // Free the gc handle if it comes from a .Set call, otherwise let the Dtor hook handle it.
                if (srcBox.ShouldFree)
                    Managed.FreeGcHandle(srcHandles[i]);
            }
        }

        private static void DefaultManagedCopy(void* dst, void* src, int count, ecs_type_info_t* typeInfoHandle)
        {
            GCHandle* dstHandles = (GCHandle*)dst;
            GCHandle* srcHandles = (GCHandle*)src;

            for (int i = 0; i < count; i++)
            {
                BindingContext.Box<T0> dstBox = (BindingContext.Box<T0>)dstHandles[i].Target!;
                BindingContext.Box<T0> srcBox = (BindingContext.Box<T0>)srcHandles[i].Target!;

                dstBox.Value = srcBox.Value!;

                // Free the gc handle if it comes from a .Set call, otherwise let the Dtor hook handle it.
                if (srcBox.ShouldFree)
                    Managed.FreeGcHandle(srcHandles[i]);
            }
        }

        private static void OnAddIterFieldCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.IterFieldCallback<T0> callback = (Ecs.IterFieldCallback<T0>)context->OnAdd.GcHandle.Target!;

            Iter it = new Iter(iter);
            for (int i = 0; i < iter->count; i++)
                callback(it, it.Field<T0>(0));
        }

        private static void OnAddIterSpanCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.IterSpanCallback<T0> callback = (Ecs.IterSpanCallback<T0>)context->OnAdd.GcHandle.Target!;

            Iter it = new Iter(iter);
            for (int i = 0; i < iter->count; i++)
                callback(it, it.GetSpan<T0>(0));
        }

        private static void OnAddIterPointerCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.IterPointerCallback<T0> callback = (Ecs.IterPointerCallback<T0>)context->OnAdd.GcHandle.Target!;

            Iter it = new Iter(iter);
            for (int i = 0; i < iter->count; i++)
                callback(it, it.GetPointer<T0>(0));
        }

        private static void OnAddEachRefCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachRefCallback<T0> callback = (Ecs.EachRefCallback<T0>)context->OnAdd.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(ref Managed.GetTypeRef<T0>(&pointer[i]));
        }

        private static void OnAddEachEntityRefCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachEntityRefCallback<T0> callback = (Ecs.EachEntityRefCallback<T0>)context->OnAdd.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(new Entity(iter->world, iter->entities[i]), ref Managed.GetTypeRef<T0>(&pointer[i]));
        }

        private static void OnAddEachIterRefCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachIterRefCallback<T0> callback = (Ecs.EachIterRefCallback<T0>)context->OnAdd.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(new Iter(iter), i, ref Managed.GetTypeRef<T0>(&pointer[i]));
        }

        private static void OnAddEachPointerCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachPointerCallback<T0> callback = (Ecs.EachPointerCallback<T0>)context->OnAdd.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(&pointer[i]);
        }

        private static void OnAddEachEntityPointerCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachEntityPointerCallback<T0> callback = (Ecs.EachEntityPointerCallback<T0>)context->OnAdd.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(new Entity(iter->world, iter->entities[i]), &pointer[i]);
        }

        private static void OnAddEachIterPointerCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachIterPointerCallback<T0> callback = (Ecs.EachIterPointerCallback<T0>)context->OnAdd.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(new Iter(iter), i, &pointer[i]);
        }

        private static void OnSetIterFieldCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.IterFieldCallback<T0> callback = (Ecs.IterFieldCallback<T0>)context->OnSet.GcHandle.Target!;

            Iter it = new Iter(iter);
            for (int i = 0; i < iter->count; i++)
                callback(it, it.Field<T0>(0));
        }

        private static void OnSetIterSpanCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.IterSpanCallback<T0> callback = (Ecs.IterSpanCallback<T0>)context->OnSet.GcHandle.Target!;

            Iter it = new Iter(iter);
            for (int i = 0; i < iter->count; i++)
                callback(it, it.GetSpan<T0>(0));
        }

        private static void OnSetIterPointerCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.IterPointerCallback<T0> callback = (Ecs.IterPointerCallback<T0>)context->OnSet.GcHandle.Target!;

            Iter it = new Iter(iter);
            for (int i = 0; i < iter->count; i++)
                callback(it, it.GetPointer<T0>(0));
        }

        private static void OnSetEachRefCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachRefCallback<T0> callback = (Ecs.EachRefCallback<T0>)context->OnSet.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(ref Managed.GetTypeRef<T0>(&pointer[i]));
        }

        private static void OnSetEachEntityRefCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachEntityRefCallback<T0> callback = (Ecs.EachEntityRefCallback<T0>)context->OnSet.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(new Entity(iter->world, iter->entities[i]), ref Managed.GetTypeRef<T0>(&pointer[i]));
        }

        private static void OnSetEachIterRefCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachIterRefCallback<T0> callback = (Ecs.EachIterRefCallback<T0>)context->OnSet.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(new Iter(iter), i, ref Managed.GetTypeRef<T0>(&pointer[i]));
        }

        private static void OnSetEachPointerCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachPointerCallback<T0> callback = (Ecs.EachPointerCallback<T0>)context->OnSet.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(&pointer[i]);
        }

        private static void OnSetEachEntityPointerCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachEntityPointerCallback<T0> callback = (Ecs.EachEntityPointerCallback<T0>)context->OnSet.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(new Entity(iter->world, iter->entities[i]), &pointer[i]);
        }

        private static void OnSetEachIterPointerCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachIterPointerCallback<T0> callback = (Ecs.EachIterPointerCallback<T0>)context->OnSet.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(new Iter(iter), i, &pointer[i]);
        }

        private static void OnRemoveIterFieldCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.IterFieldCallback<T0> callback = (Ecs.IterFieldCallback<T0>)context->OnRemove.GcHandle.Target!;

            Iter it = new Iter(iter);
            for (int i = 0; i < iter->count; i++)
                callback(it, it.Field<T0>(0));
        }

        private static void OnRemoveIterSpanCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.IterSpanCallback<T0> callback = (Ecs.IterSpanCallback<T0>)context->OnRemove.GcHandle.Target!;

            Iter it = new Iter(iter);
            for (int i = 0; i < iter->count; i++)
                callback(it, it.GetSpan<T0>(0));
        }

        private static void OnRemoveIterPointerCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.IterPointerCallback<T0> callback = (Ecs.IterPointerCallback<T0>)context->OnRemove.GcHandle.Target!;

            Iter it = new Iter(iter);
            for (int i = 0; i < iter->count; i++)
                callback(it, it.GetPointer<T0>(0));
        }

        private static void OnRemoveEachRefCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachRefCallback<T0> callback = (Ecs.EachRefCallback<T0>)context->OnRemove.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(ref Managed.GetTypeRef<T0>(&pointer[i]));
        }

        private static void OnRemoveEachEntityRefCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachEntityRefCallback<T0> callback = (Ecs.EachEntityRefCallback<T0>)context->OnRemove.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(new Entity(iter->world, iter->entities[i]), ref Managed.GetTypeRef<T0>(&pointer[i]));
        }

        private static void OnRemoveEachIterRefCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachIterRefCallback<T0> callback = (Ecs.EachIterRefCallback<T0>)context->OnRemove.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(new Iter(iter), i, ref Managed.GetTypeRef<T0>(&pointer[i]));
        }

        private static void OnRemoveEachPointerCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachPointerCallback<T0> callback = (Ecs.EachPointerCallback<T0>)context->OnRemove.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(&pointer[i]);
        }

        private static void OnRemoveEachEntityPointerCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachEntityPointerCallback<T0> callback = (Ecs.EachEntityPointerCallback<T0>)context->OnRemove.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(new Entity(iter->world, iter->entities[i]), &pointer[i]);
        }

        private static void OnRemoveEachIterPointerCallback(ecs_iter_t* iter)
        {
            BindingContext.TypeHooksContext* context = (BindingContext.TypeHooksContext*)iter->callback_ctx;
            Ecs.EachIterPointerCallback<T0> callback = (Ecs.EachIterPointerCallback<T0>)context->OnRemove.GcHandle.Target!;

            T0* pointer = (T0*)iter->ptrs[0];
            for (int i = 0; i < iter->count; i++)
                callback(new Iter(iter), i, &pointer[i]);
        }
    }
}
