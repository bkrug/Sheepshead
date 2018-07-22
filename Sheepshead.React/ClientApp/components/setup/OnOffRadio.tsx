import '../../css/setup.css';
import * as React from 'react';

export interface OnOffState {
    value: boolean;
}

export interface OnOffProps {
    onText: string;
    offText: string;
    disabled: boolean;
    name: string;
    title: string;
    defaultValue: boolean;
}

export default class OnOffRadio extends React.Component<OnOffProps, OnOffState> {
    constructor(props: OnOffProps) {
        super(props);
        this.state = {
            value: props.defaultValue
        }
        this.handleClick = this.handleClick.bind(this);
    }

    handleClick(i: boolean) {
        this.setState({
            value: i
        });
    }

    public render() {
        return (
            <div className="onOffRadio">
                <div className="title-container"><span>{this.props.title}</span></div>
                <div className="button-container">
                    <span>
                        <input
                            type="radio"
                            name={this.props.name}
                            value={this.props.onText}
                            onClick={() => this.handleClick(true)}
                            checked={this.state.value}
                            disabled={this.props.disabled}
                        />{this.props.onText}
                    </span>
                    <span>
                        <input
                            type="radio"
                            name={this.props.name}
                            value={this.props.offText}
                            onClick={() => this.handleClick(false)}
                            checked={!this.state.value}
                            disabled={this.props.disabled}
                        />{this.props.offText}
                    </span>
                </div>
                <input type="hidden" className="value" value={this.state.value ? 'on' : 'off'} />
            </div>
        );
    }
}