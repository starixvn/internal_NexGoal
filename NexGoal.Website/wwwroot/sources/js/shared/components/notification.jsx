import React, { Component, Fragment } from 'react';

export default class Notification extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showSuccess: false,
            showError: false,
            successMessage: '',
            errorMessage: ''
        };
    }

    componentDidUpdate() {
        if (this.state.showSuccess) {
            setTimeout(() => this.setState({ showSuccess: false }), 2000);
        }

        if (this.state.showError) {
            setTimeout(() => this.setState({ showError: false }), 2000);
        }
    }

    showSuccess = (message) => {
        this.setState({ showSuccess: true, successMessage: message });
    }

    showError = (message) => {
        this.setState({ errorMessage: true, errorMessage: message });
    }

    render() {
        return (
            <Fragment>
                <div className={`msg-success -has-border ${this.state.showSuccess ? '' : 'hidden'}`}>
                    {this.state.successMessage}
                </div>
                <div className={`-has-border msg-error ${this.state.showError ? '' : 'hidden'}`}>
                    {this.state.errorMessage}
                </div>
            </Fragment >
        )
    }
}