import React, { PureComponent } from 'react';
import memoize from 'memoize-one';

export default class Marquee extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            run: true
        };
    }

    toggleRun = () => this.setState(prevState => ({ run: !prevState.run }));

    renderMessage = (message, index) => (
        <span key={`message${index}`} className="announcement-item">
            {message}
        </span>)

    getStyle = memoize(
        (messages, run) => {
            const duration = messages.reduce((total, current) => total + current.length, 0) / 5;
            const animation = `marquee ${duration}s linear infinite ${run ? 'running' : 'paused'}`;

            return {
                WebkitAnimation: animation,
                KhtmlAnimation: animation,
                MozAnimation: animation,
                msAnimation: animation,
                OAnimation: animation,
                animation: animation
            };
        }
    );

    render() {
        const style = this.getStyle(this.props.messages, this.state.run);

        return (
            <div className="marquee-block">
                <div className={`btn-pause ${!this.state.run && 'play'}`} onClick={this.toggleRun} />
                <div className="marquee" onClick={this.props.onClick} >
                    <span className="annoucement" style={style}>
                        {
                            this.props.messages.map(this.renderMessage)
                        }
                    </span>
                </div>
            </div>

        );
    }
}