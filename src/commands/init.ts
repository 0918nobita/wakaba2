import * as E from 'fp-ts/Either';
import * as TE from 'fp-ts/TaskEither';
import { pipe } from 'fp-ts/function';
import fs from 'fs';
import { prompt } from 'inquirer';
import path from 'path';

const makeQuestion =
    <T extends string>(type: 'input' | 'password', name: T, message?: string) =>
        pipe(
            TE.tryCatch(
                (): Promise<{ [_ in T]: string }> =>
                    prompt(message ? { type, name, message } : { type, name }),
                String
            ),
            TE.map(v => v[name])
        );

const questions =
    pipe(
        makeQuestion('input', 'executablePath', 'Path to the executable of Chrome browser'),
        TE.chain((executablePath) =>
            pipe(
                makeQuestion('input', 'username'),
                TE.chain((username) =>
                    pipe(
                        makeQuestion('password', 'password'),
                        TE.map((password) => ({ executablePath, username, password }))
                    )
                )
            )
        )
    );

export const init = async (configFilePath: string): Promise<void> => {
    const result = await questions();

    pipe(
        result,
        E.map(({ executablePath, username, password }) => {
            fs.writeFileSync(
                configFilePath,
                JSON.stringify({ executablePath, ouj: { username, password } })
            );
        })
    );
};
