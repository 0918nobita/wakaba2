declare module 'chrome-remote-interface' {
    interface Network {
        enable(): Promise<void>;
        setUserAgentOverride(options: { userAgent: string }): Promise<void>;
    }

    interface Page {
        enable(): Promise<void>;
        loadEventFired(): Promise<void>;
        navigate(options: { url: string }): Promise<void>;
    }

    interface CDPClient {
        Network: Network;
        Page: Page;
    }

    function CDP(): Promise<CDPClient>;
    export = CDP;
}
