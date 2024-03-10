namespace Hakuu.Base
{
    internal enum EventType
    {
        ServerStart,
        ServerStop,
        ServerExitUnexpectedly,
        ServerOutput,
        ServerOriginalOutput,
        ServerSendCommand,

        GroupIncrease,
        GroupDecrease,
        GroupPoke,
        ReceiveGroupMessage,
        ReceivePrivateMessage,
        ReceivePacket,
        PermissionDeniedFromPrivateMsg,
        PermissionDeniedFromGroupMsg,

        HakuuClose,
        HakuuCrash,

        PluginsReload,
        PluginsLoaded,

        BindingSucceed,
        BindingFailDueToOccupation,
        BindingFailDueToInvalid,
        BindingFailDueToAlreadyBinded,
        UnbindingSucceed,
        UnbindingFail,
        BinderDisable,

        RequestingMotdpeSucceed,
        RequestingMotdjeSucceed,
        RequestingMotdFail
    }
}
