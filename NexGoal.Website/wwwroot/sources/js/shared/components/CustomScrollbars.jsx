import React, { Component } from 'react';
import { Scrollbars } from 'react-custom-scrollbars';

export default class CustomScrollbars extends Component {
    constructor(props) {
        super(props);

        this.state = {
            hover: false
        };
    }

    handleMouseEnter = () => {
        this.setState({ hover: true });
    };

    handleMouseLeave = () => {
        this.setState({ hover: false });
    };

    renderTrackVertical = ({ style, ...props }) => {
        const { hover } = this.state;
        return (
            <div className="track-vertical" style={{
                    ...style,
                    borderRadius: 3,
                    bottom: 2,
                    right: 2,
                    top: 2,
                    opacity: hover ? 1 : 0
                }} {...props}
            />
        );
    };

    renderTrackHorizontal = ({ style, ...props }) => {
        const { hover } = this.state;
        return (
            <div style={{
                    ...style,
                    borderRadius: 3,
                    bottom: 2,
                    right: 2,
                    left: 2,
                    opacity: hover ? 1 : 0
                }} {...props}
            />
        );
    };

    render() {
        return (
            <Scrollbars className="area-scroll" style={{ height: '100%' }}
                renderTrackHorizontal={this.renderTrackHorizontal}
                renderTrackVertical={this.renderTrackVertical}
                onMouseEnter={this.handleMouseEnter}
                onMouseLeave={this.handleMouseLeave}
                {...this.props}
            />
        );
    }
}