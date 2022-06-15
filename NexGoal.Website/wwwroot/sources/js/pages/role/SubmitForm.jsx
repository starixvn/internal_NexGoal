import React from 'react';
import { Button, FormGroup } from 'react-bootstrap';

const SubmitForm = ({ isHost, onSubmit }) => {
    return (
        isHost &&
        <form className="form-inline form-filter">
            <FormGroup>
                <Button type="button" bsStyle="primary" onClick={onSubmit}>Submit</Button>
            </FormGroup>
        </form>
    );
};

export default SubmitForm;