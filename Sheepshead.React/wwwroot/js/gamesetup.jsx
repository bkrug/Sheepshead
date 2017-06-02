class GameSetup extends React.Component {
    constructor(props) {
        super(props);
        this.setConstants();
        this.state = { value: 0, remaining: this.MAX_PLAYERS };
        this.handleChange = this.handleChange.bind(this);
        this.selections = {};
        this.selections[this.HUMANS] = this.selections[this.NEWBIE] = this.selections[this.BASIC] = this.selections[this.LEARNING] = 0;
    }

    setConstants() {
        this.MAX_PLAYERS = 5;
        this.HUMANS = "humans";
        this.NEWBIE = "newbie";
        this.BASIC = "basic";
        this.LEARNING = "learning";
    }

    handleChange(radioGroup, radioValue) {
        if (radioValue === 0 || radioValue > 0) {
            this.selections[radioGroup] = radioValue;
            var newTotal = this.selections[this.HUMANS] + this.selections[this.NEWBIE] + this.selections[this.BASIC] + this.selections[this.LEARNING];
            this.setState({ value: newTotal, remaining: this.MAX_PLAYERS - newTotal });
        }
    }

    playerCountValidityClass() {
        return (this.state.value === 3 || this.state.value === 5) ? '' : 'invalidcount';
    }

    render() {
        return (
            <div className="gameSetup">
                <h4>Setup Sheepshead Game</h4>
                <PlayerCountRadio name={this.HUMANS} title="Humans" onChange={this.handleChange} remaining={this.state.remaining} />
                <PlayerCountRadio name={this.NEWBIE} title="A.I. Simple" onChange={this.handleChange} remaining={this.state.remaining} />
                <PlayerCountRadio name={this.BASIC} title="A.I. Basic" onChange={this.handleChange} remaining={this.state.remaining} />
                <PlayerCountRadio name={this.LEARNING} title="A.I. Statistic" onChange={this.handleChange} remaining={this.state.remaining} />
                <div>
                    <span>Total Players:</span>
                    <span className={"totalPlayers " + this.playerCountValidityClass()}>{this.state.value}</span>
                </div>
                <input type="hidden" className="remaining" value={this.state.remaining} />
                <input type="button" value="Play" />
            </div>
        );
    }
}