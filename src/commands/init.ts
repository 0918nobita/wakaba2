import fs from 'fs';
import { prompt } from 'inquirer';

const makeQuestion =
    <T extends string>(type: 'input' | 'password', name: T, message?: string): Promise<{ [_ in T]: string }> =>
        prompt({ type, name, message });

export const init = async (configFilePath: string) => {
    const { executablePath } = await makeQuestion('input', 'executablePath', '');
    const { username } = await makeQuestion('input', 'username');
    const { password } = await makeQuestion('password', 'password');

    fs.writeFileSync(
        configFilePath,
        JSON.stringify({ executablePath, ouj: { username, password } })
    );
};
