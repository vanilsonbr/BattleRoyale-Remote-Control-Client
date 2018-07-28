# BattleRoyale-Remote-Control-Client
Desafio lançado pelo Instituto Atlântico. Gerenciamento de máquinas clientes via linha de comando através de um servidor Web. Versão do Client.

Este projeto representa um serviço criado para execução de comandos remotamente pelo servidor (https://github.com/vanilsonbr/BattleRoyale-Remote-Control)

## Executando o código-fonte:
#### Para executar o código-fonte, você precisará das seguintes ferramentas:
* Visual Studio 2013 ou superior
* .Net Framework 3.5 ou superior
* Permissão de administrador para instalar o serviço

1. Clone este projeto
2. Abra o projeto no Visual Studio em modo adminstrador e compile a Solução
3. Abra um prompt de comando em modo administrador na pasta /bin/Debug da solução
4. Execute **InstallUtil.exe BatteRoyale.RemoteController.Client.exe** para instalar o serviço
5. Execute **InstallUtil.exe /u BatteRoyale.RemoteController.Client.exe** para desinstalar o serviço

*Importante*: Desinstale o serviço antes de compilar novamente a Solução.

## Instalando o serviço sem necessidade do Visual Studio
* Baixe a pasta **Instalação** no diretório raiz deste repositório
* Clique com o botão direito no arquivo **Instalar Servico BatteRoyale.bat** e selecione **Executar como administrador**
* Uma tela da linha de comando abrirá, instalará o serviço e o ativará.
* Aperte qualquer botão para sair da instalação.

## Testando o serviço

Após a instalação do serviço, ele estará rodando em background e ouvindo as solicitações do servidor.

É importante notar que, se o Servidor não estiver executando ou DNS dele estiver errado, o serviço não conseguirá receber requisições HTTP
dele e não aparecerá na sua lista de clientes cadastrados.

Mesmo assim, ainda é possível testar o serviço fazendo requisições HTTP para ele e esperando o resultado.

* Abra um software de testes de requisições HTTP, como o Postman, SoapUI, ou mesmo o browser para testar.
* Verifique qual é seu **IP local**: abra a linha de comando digite **ipconfig** e copie o **Endereço IPv4**
* Faça uma requisição GET para http://<IPLocal>/handshake, deve retornar true
* Faça uma requisição POST para http://<IPLocal>/receivecommand passando um comando (tal como "dir"), deve retornar um json com o resultado.
* Faça uma requisição POST para http://<IPLocal>/clientinformation e você receberá os dados coletados da máquina




