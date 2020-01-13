import React, { PureComponent } from 'react';
import { Button, FormGroup } from 'react-bootstrap';

export default class SubmitForm extends PureComponent {
    render() {
        return (
            this.props.isHost &&
            <form className="form-inline form-filter">
                <FormGroup>
                    <Button type="button" bsStyle="primary" onClick={this.props.onSubmit}>Submit</Button>
                </FormGroup>
            </form>
        );
    }
}