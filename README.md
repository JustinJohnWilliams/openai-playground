## Setup
- create free account with [OpenAI](https://openai.com/api/) to grab an API Key
- `export OPEN_AI_KEY=<your API KEY>`
- `dotnet publish`
- `sudo ln -s ~/<path to playground>/bin/Release/net6.0/publish/openai-playground /usr/local/bin/openai-playground`

## Customize
- read through [docs](https://platform.openai.com/docs/introduction) to change [these lines](https://github.com/JustinJohnWilliams/openai-playground/blob/bc357989460d07decbf8b84df31c6f19708cbdc6/openai-playground/Program.cs#L40-L48) for using different models

## Examples
### Limerick
`openai-playground limerick a boy named sue`

### Code
`openai-playground code C# parallel process multiple CSV files`

### Pictures
`openai-playground pic a unicorn riding a motorcycle`

### Query
`openai-playground write me a 300 word essay on the economic stability of Greece`

## Bash Functions
```bash
function limerick() {
  local LIMERICK=$(openai-playground limerick "$@")
  echo $LIMERICK | pbcopy
  echo $LIMERICK
}

function dirty-limerick() {
  local LIMERICK=$(openai-playground dirty-limerick "$@")
  echo $LIMERICK | pbcopy
  echo $LIMERICK
}

function how-do-i-code() {
  local HOW_DO_I=$(openai-playground code "$@")
  echo $HOW_DO_I | pbcopy
  echo $HOW_DO_I
}

function oai() {
  local RESULT=$(openai-playground "$@")
  echo $RESULT | pbcopy
  echo $RESULT
}

function oai-pic() {
  RESULT=$(openai-playground pic "$@")
  echo $RESULT | pbcopy
  if [[ $RESULT == *"ERROR"* ]]; then
    echo $RESULT
    return
  fi
  open $(echo $RESULT)
}
```