import React, { useState, useEffect, useRef } from 'react';
import Terminal from 'terminal-in-react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

const MyTerminal = () => {
    const [connection, setConnection] = useState();
    const [headerMessage, setHeaderMessage] = useState("Initializing...");
    const terminalPrint = useRef();
    const [connected, setConnected] = useState(false);

    useEffect(() => {
        const hubUrl = process.env.REACT_APP_HUB_URL;
        const connection = new HubConnectionBuilder()
            .withUrl(`${hubUrl}/cmdhub`)
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: (ctx) => {
                    return 5000;
                }
            })
            .configureLogging(LogLevel.Trace)
            .build();

        setConnection(connection);

        return () => {
            connection.stop();
        }
    }, []);

    useEffect(() => {
        if (connection) {
            
            connection
                .start()
                .then(() => {
                    setHeaderMessage("Connected with server.");
                    connection.on('ReceiveResponse', receivedMessage => {
                        console.log(receivedMessage);
                        if (terminalPrint.current) {
                            terminalPrint.current(receivedMessage);
                        }
                    });
                    setConnected(true);
                })
                .catch(err => {
                    setHeaderMessage("Cannot connect with server.");
                });

            connection.onclose(() => setConnected(false));
            connection.onreconnected(() => setConnected(true));
        }
    }, [connection]);

    const handleCommand = (cmd, print, c) => {
        terminalPrint.current = print;

        if (connection._connectionStarted) {
            console.log(cmd);
            try {
                let data = {
                    connectionId: connection.connectionId,
                    command: cmd.join(' ')
                };
                
                connection.send('SendCommand', data)
                    .then((e) => {
                        console.log(e);
                    });
            }
            catch (e) {
                console.log(e);
            }
        }
        else {
            setHeaderMessage("Connection lost with server.");
        }
    }

    return (
        <>
            {!connected && <div>Connecting</div>}

            { connected && <Terminal
                color="black"
                backgroundColor="white"
                barColor="white"
                msg={headerMessage}
                style={{height: '100vh', lineHeight: '.8rem', color: 'black', fontSize: '1.3rem'}}
                commandPassThrough={handleCommand}
                startState="maximised"
            />}
        </>
    );
};

export default MyTerminal;