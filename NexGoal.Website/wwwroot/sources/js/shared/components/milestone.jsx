import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import isEqual from 'react-fast-compare';

export default class Milestone extends Component {
    static propTypes = {
        options: PropTypes.arrayOf(PropTypes.object),
        onChange: PropTypes.func
    };

    static defaultProps = {
        options: [],
        onChange: function () {
            //empty
        }
    };

    constructor(props) {
        super(props);

        this.state = {
            currentIndex: 0,
            maxIndex: props.options.length - 1,
            nextDisabled: true,
            prevDisabled: props.options.length <= 1
        };
    }

    shouldComponentUpdate(nextProps, nextState) {
        return !isEqual(this.state, nextState) || !isEqual(this.props, nextProps);
    }

    onClickComplete = () => {
        this.setState({
            nextDisabled: this.state.currentIndex === 0,
            prevDisabled: this.state.currentIndex === this.state.maxIndex
        });
        this.props.onChange(this.props.options[this.state.currentIndex].value);
    };

    onClickNext = () => {
        if (!this.state.nextDisabled) {
            this.setState({
                currentIndex: this.state.currentIndex - 1
            }, this.onClickComplete);
        }
    };

    onClickPrev = () => {
        if (!this.state.prevDisabled) {
            this.setState({
                currentIndex: this.state.currentIndex + 1
            }, this.onClickComplete);
        }
    };

    render() {
        const options = this.props.options;
        const nextDisabled = this.state.nextDisabled ? 'disabled' : '';
        const prevDisabled = this.state.prevDisabled ? 'disabled' : '';

        return (
            <div className="milestone">
                <span className={`icon-chevron-left arrow-left ${prevDisabled}`}
                    disabled={this.state.prevDisabled}
                    onClick={this.onClickPrev}
                />
                {
                    options.length > 0 &&
                    <Fragment>{options[this.state.currentIndex].name}</Fragment>
                }
                <span className={`icon-chevron-right arrow-right ${nextDisabled}`}
                    disabled={this.state.nextDisabled}
                    onClick={this.onClickNext}
                />
            </div>
        );
    }
}