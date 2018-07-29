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

O Serviço foi instalado no sistema e vai logar qualquer requisição que receber, assim como quaisquer erros que surjam no Log de Eventos do Windows.

## Testando o serviço

Após a instalação do serviço, ele estará rodando em background e ouvindo as solicitações do servidor.

É importante notar que, se o Servidor não estiver executando ou DNS dele estiver errado, o serviço não conseguirá receber requisições HTTP
dele e não aparecerá na sua lista de clientes cadastrados.

Mesmo assim, ainda é possível testar o serviço fazendo requisições HTTP para ele e esperando o resultado.

* Abra um software de testes de requisições HTTP, como o Postman, SoapUI, ou mesmo o browser para testar.
* Verifique qual é seu **IP local**: abra a linha de comando digite **ipconfig** e copie o **Endereço IPv4**
* Faça uma requisição GET para http://{SeuIPLocal}/handshake, deve retornar true
* Faça uma requisição POST para http://{SeuIPLocal}/receivecommand passando um comando (tal como "dir"), deve retornar um json com o resultado.
* Faça uma requisição POST para http://{SeuIPLocal}/clientinformation e você receberá os dados coletados da máquina

## À equipe de avaliação do Instituto Atlântico
Desenvolver este projeto em Windows Services usando um listener simples de requisições HTTP foi desafiador para mim. Gostaria de agradecer pela oportunidade de me proporcionarem este desafio. É sempre bom por meu conhecimento em prática e engrandecer cada vez mais meu trabalho.

Embora esta ainda seja a primeira versão e ainda hajam muitas melhorias que eu gostaria de fazer caso tivesse tempo, acredito que
consegui chegar a um bom nivel.

Alguns pontos de melhoria que vejo neste momento para este Serviço:
* **Usar WebSockets ao invés de requisições HTTP**: no segundo dia do desafio, com algumas partes do projeto Web implementados, eu tentei uma implementação de WebSockets. Infelizmente não pude continuar com essa abordagem pois minha máquina não bate os requisitos minimos. Então tive que usar a abordagem atual.
* **Melhoria do HttpHandler**: embora existam bibliotecas e Wrappers para requisições HTTP para aplicações de console e serviços do Windows já implementadas por aí, eu resolvi fazer uma implementação própria para escutar o Servidor, visto que um handler simples para este projeto seria suficiente. De qualquer forma, se o projeto tivesse mais versões e eventualmente fosse usado por mais pessoas, eu optaria ou por melhorar o Handler ou usar um Wrapper/Biblioteca mais robusto.
* **Testes mais aprofundados**: Os testes que fiz até o presente momento foram todos feitos já fazendo chamadas às rotas e verificando os resultados e logs, o que não configura um teste unitário. Pretendo fazer testes melhores para este projeto e já iniciar futuros projetos deste tipo com TDD.
* **Organização melhor do código**: neste projeto, tentei ao máximo delegar as responsabilidades corretamente para cada classe dependendo de sua funcionalidade. Por exemplo: o RemoteControlHttpHandler apenas recebe as requisições e seus parametros e passa para o RemoteControlHttpHandlerExecuter para que ele tome a decisão de o que enviar como resultado, a própria Model Client recupera as informações da máquina quando um objeto seu é instanciado, etc. Tentei usar tambem padrões de projeto que aprendi ao longo da minha carreira para que a codificaçao e o entendimento ficasse mais simples e escalável (como no caso do RemoteControlHttpHandler que atua como um builder ao adicionar listeners). Mesmo assim, ainda vejo muitas melhorias que poderia aplicar a este código para que fique cada vez melhor.

Espero que tenham gostado do meu código! Qualquer dúvida, reclamação, crítica, estou à disposição para ouvir. É sempre bom saber como está meu trabalho através de um feedback. :)

## Referências usadas para desenvolver o serviço:
* https://docs.microsoft.com/pt-br/dotnet/framework/windows-services/walkthrough-creating-a-windows-service-application-in-the-component-designer
* https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/sc-create
* https://gist.github.com/aksakalli/9191056 (rodando e parando listener em uma thread adequadamente)


