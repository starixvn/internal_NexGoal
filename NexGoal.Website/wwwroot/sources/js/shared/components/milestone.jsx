import React, { Component } from 'react';

export default class Milestone extends Component {
    constructor(props) {
        super(props);
        this.state = {
            currentIndex: 0,
            maxIndex: props.options.length - 1,
            nextDisabled: true,
            prevDisabled: props.options.length <= 1
        };
    }

    onClickNext = () => {
        this.setState({
            currentIndex: this.state.currentIndex - 1
        }, () => {
            this.setState({
                nextDisabled: this.state.currentIndex === 0,
                prevDisabled: this.state.currentIndex === this.state.maxIndex
            });
            this.props.onSelectionChange(this.props.options[this.state.currentIndex].value);
        });
    }

    onClickPrev = () => {
        this.setState({
            currentIndex: this.state.currentIndex + 1
        }, () => {
            this.setState({
                nextDisabled: this.state.currentIndex === 0,
                prevDisabled: this.state.currentIndex === this.state.maxIndex
            });
            this.props.onSelectionChange(this.props.options[this.state.currentIndex].value);
        });
    }

    render() {
        const options = this.props.options;
        const nextDisabled = this.state.nextDisabled ? 'disabled' : '';
        const prevDisabled = this.state.prevDisabled ? 'disabled' : '';
        return (
            <div className="milestone">
                <span className={`icon-chevron-left arrow-left ${prevDisabled}`}
                    disabled={this.state.prevDisabled}
                    onClick={this.onClickPrev}
                ></span>
                {options[this.state.currentIndex].name}
                <span className={`icon-chevron-right arrow-right ${nextDisabled}`}
                    disabled={this.state.nextDisabled}
                    onClick={this.onClickNext}
                ></span>
            </div>
        );
    }
}