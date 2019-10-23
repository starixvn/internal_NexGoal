import React, { PureComponent } from 'react';

export default class StickyBlock extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            showStickyHeader: false
        };
    }

    componentDidMount() {
        window.addEventListener('scroll', this.handleToggleSticky);
    }

    componentWillUnmount() {
        window.removeEventListener('scroll', this.handleToggleSticky);
    }

    handleToggleSticky = () => {
        if (window.scrollY > this.props.threshold) {
            if (!this.state.showStickyHeader) {
                this.setState({ showStickyHeader: true });
            }
        } else {
            if (this.state.showStickyHeader) {
                this.setState({ showStickyHeader: false });
            }
        }
    }

    render() {
        return (
            <div className={`sticky-block ${this.state.showStickyHeader ? '' : 'hidden'}`}>
                {this.props.children}
            </div>
        );
    }
}